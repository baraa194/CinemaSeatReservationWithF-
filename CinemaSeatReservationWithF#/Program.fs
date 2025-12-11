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

    /// Prompt for integer input
    let rec promptInt (message:string) : int option =
        printf "%s" message
        match Console.ReadLine() with
        | null -> None
        | s when s.Trim().ToLower() = "q" -> None
        | s ->
            match Int32.TryParse(s.Trim()) with
            | true, v -> Some v
            | false, _ ->
                printfn "Invalid input. Please enter a number or 'q'."
                promptInt message

    /// Register & login user
    let testRegisterAndLogin () =
        printfn "=== Register / Login ==="
        printf "Enter a username: "
        let username = Console.ReadLine()
        printf "Enter an email: "
        let email = Console.ReadLine()
        printf "Enter a password: "
        let password = Console.ReadLine()

        match UserService.registerUser username email password with
        | Ok newId -> printfn "✔ Registered. UserId=%d" newId
        | Error msg -> printfn "✘ Register failed: %s" msg

        match UserService.loginUser username password with
        | Success user -> printfn "✔ Logged in. Welcome %s (id=%d)" user.Username user.Id; Some user
        | InvalidCredentials -> printfn "✘ Login failed: invalid credentials."; None
        | NotFound -> printfn "✘ Login failed: user not found."; None

    [<EntryPoint>]
    let main argv =
        try
            //  Initialize database
            runInitSql ()

            //  Register & login user
            match testRegisterAndLogin () with
            | None ->
                printfn "Cannot continue without login."
                1
            | Some user ->

                //  Show movies
                let movies = MoviesService.getAllMovies()
                printfn "\nAvailable Movies:"
                movies |> List.iter (fun m -> printfn "%d: %s (%d min)" m.Id m.Title m.DurationMinutes)

                //  Prompt user to select movie
                let movieId =
                    match promptInt "\nEnter Movie ID to see screenings: " with
                    | Some id -> id
                    | None -> failwith "No movie selected"

                //  Show screenings for selected movie
                let screenings = ScreeningService.getScreeningsForMovie movieId
                if List.isEmpty screenings then
                    printfn "No screenings for this movie."
                    1
                else
                    printfn "\nAvailable Screenings:"
                    screenings
                    |> List.iter (fun s -> printfn "%d: Hall %d at %O" s.Id s.HallId s.StartAt)

                    //  Prompt user to select screening
                    let screeningId =
                        match promptInt "\nEnter Screening ID to book: " with
                        | Some id -> id
                        | None -> failwith "No screening selected"

                    //  Load screening and hall
                    let screeningOpt = ScreeningService.getScreeningById screeningId
                    match screeningOpt with
                    | None ->
                        printfn "Screening not found. Exiting."
                        1
                    | Some screening ->
                        let hallOpt = HallService.getHallById screening.HallId
                        match hallOpt with
                        | None ->
                            printfn "Hall not found. Exiting."
                            1
                        | Some hall ->
                            let hallId = hall.Id
                            let rows = hall.RowsCount
                            let cols = hall.ColsCount

                            let seats = SeatService.getAvailableSeatsForHall hallId
                            printfn "\nAvailable seats in Hall %d:" hallId
                            printfn "%d" (List.length seats)

                            printfn "\nSeat layout for Hall %d:" hallId
                            SeatService.renderMatrix hallId rows cols

                            // Prompt user to pick seat
                            match promptInt "\nEnter Seat ID to book (or 'q' to quit): " with
                            | None ->
                                printfn "No seat selected. Exiting."
                                0
                            | Some seatId ->
                                match SeatService.tryBookSeat seatId screeningId with
                                | Some ticketId ->
                                    printfn "✔ Seat %d booked! Ticket ID: %A" seatId ticketId
                                    0
                                | None ->
                                    printfn "✘ Failed to book seat %d." seatId
                                    1

        with ex ->
            printfn "Unhandled exception: %s" ex.Message
            1
