namespace CinemaSeatReservationWithFSharp

open System
open System.IO
open CinemaSeatReservationWithFSharp.Services
open CinemaSeatReservationWithFSharp.Repositories
open CinemaSeatReservationWithFSharp.Db
open CinemaSeatReservationWithFSharp.Repositories.UserRepo
open Dapper

module Program =
    
    let runInitSql () =
        use conn = getConnection()
        conn.Open()
        let sql = File.ReadAllText("init.sql")
        conn.Execute(sql) |> ignore
        printfn "Database initialized successfully."

    /// Prompt the user until they enter a valid integer seat id
    let rec promptSeatId () : int option =
        printf "\nEnter seat id to book : "
        match Console.ReadLine() with
        | null -> None
        | s when s.Trim().ToLower() = "q" -> None
        | s ->
            match Int32.TryParse(s.Trim()) with
            | true, v -> Some v
            | false, _ ->
                printfn "Invalid input. Please enter a seat number or 'q'."
                promptSeatId()

    /// Test register + login flow (user enters inputs)
    let testRegisterAndLogin () =
        printfn "=== Register / Login Test ==="

        // Ask user for username
        printf "Enter a username to register: "
        let username = Console.ReadLine()

        printf "Enter an email: "
        let email = Console.ReadLine()

        // Ask user for password
        printf "Enter a password: "
        let password = Console.ReadLine()

        printfn "\n--- Registering user... ---"

        match UserService.registerUser username email password with
        | Ok newId ->
            printfn "✔ Register succeeded. New user id = %d" newId
        | Error msg ->
            printfn "✘ Register failed: %s" msg

        printfn "\n--- Testing login... ---"

        match UserService.loginUser username password with
        | Success user ->
            printfn "✔ Login succeeded. Welcome %s (id=%d)" user.Username user.Id
        | InvalidCredentials ->
            printfn "✘ Login failed: invalid credentials."
        | NotFound ->
            printfn "✘ Login failed: user not found."

        printfn "=== End of Register/Login test ===\n"

    [<EntryPoint>]
    let main argv =
        try
            // Initialize database first
            runInitSql ()

            // Run register/login test first
            testRegisterAndLogin ()

            // Load all seats and print count
            let seats = SeatService.loadAllSeats()
            printfn "Loaded %d seats from DB" (List.length seats)

            // Get summary (available, booked)
            let (avail, booked) = SeatService.getSummary()
            printfn "Available: %d, Booked: %d" avail booked

            // Render seat matrix
            let matrix : byte[,] = SeatService.buildMatrix seats
            SeatService.renderConsole matrix

            // Ask user for seat id
            match promptSeatId () with
            | None ->
                printfn "No seat id provided. Exiting."
            | Some seatIdToTryCreate ->
                let (ticketOpt, refreshedAfterCreate) =
                    SeatService.tryBookSeatAndCreateTicketService seatIdToTryCreate

                match ticketOpt with
                | Some id ->
                    printfn "Seat %d booked and ticket created: %A" seatIdToTryCreate id
                    let availableSeats = SeatService.getavalSaets()
                    printfn "Available seats after booking: %d" (List.length availableSeats)
                | None ->
                    printfn "Failed to book seat %d (maybe already booked or doesn't exist)." seatIdToTryCreate
                    let availableSeats = SeatService.getavalSaets()
                    printfn "Available seats after booking: %d" (List.length availableSeats)

            printfn "\nPress any key to exit..."
            Console.ReadKey() |> ignore
            0

        with ex ->
            printfn "Unhandled exception: %s" ex.Message
            printfn "\nPress any key to exit..."
            Console.ReadKey() |> ignore
            1
