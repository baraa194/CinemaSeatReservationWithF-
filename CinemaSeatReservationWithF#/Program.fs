namespace CinemaSeatReservationWithFSharp

open System
open CinemaSeatReservationWithFSharp.Domain
open CinemaSeatReservationWithFSharp.Repositories  

module Program =

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

    [<EntryPoint>]
    let main argv =
        try
         

            
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
            match promptSeatId() with
            | None ->
                printfn "No seat id provided. Exiting."
            | Some seatIdToTryCreate ->
                // Call the service that creates ticket and returns refreshed seats
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
