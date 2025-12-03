namespace CinemaSeatReservationWithFSharp.Domain

open System
open CinemaSeatReservationWithFSharp.Repositories

module SeatService =

    /// Load all seats from DB (delegates to repository)
    let loadAllSeats () : SeatDto list =
        SeatRepository.getAllSeats()


    /// Try to book a seat via the improved repository function that returns BookingResult.
    /// Returns (Some ticketId, refreshedSeats) on success, otherwise (None, refreshedSeats).
  
    let tryBookSeatAndCreateTicketService (seatId:int) : Guid option * SeatDto list =
     try
        match SeatRepository.tryBookSeatAndCreateTicketResult seatId with
        | SeatRepository.Booked ticketId ->
            // ticket created in DB — now append to the single log file
            let createdAt = DateTime.UtcNow
            try
                // call the TicketService helper that appends the ticket to tickets_log.txt
                TicketService.appendTicketToFile ticketId seatId createdAt |> ignore
            with
            | ex ->
                // don't fail the whole operation if file IO fails — just log
                printfn "Warning: failed to append ticket to file: %s" ex.Message

            let refreshed = SeatRepository.getAllSeats()
            (Some ticketId, refreshed)

        | SeatRepository.AlreadyBooked ->
            let refreshed = SeatRepository.getAllSeats()
            (None, refreshed)

        | SeatRepository.NotFound ->
            let refreshed = SeatRepository.getAllSeats()
            (None, refreshed)

        | SeatRepository.Error msg ->
            printfn "SeatRepository.tryBookSeatAndCreateTicketResult error: %s" msg
            let refreshed =
                try SeatRepository.getAllSeats()
                with _ -> []
            (None, refreshed)

     with ex ->
        printfn "Unhandled exception in tryBookSeatAndCreateTicketService: %s" ex.Message
        let refreshed =
            try SeatRepository.getAllSeats()
            with _ -> []
        (None, refreshed)



    /// Get a single seat by id (option)
    let getSeatById (seatId:int) : SeatDto option =
        SeatRepository.tryGetSeatById seatId


    /// Get available seats
    let getavalSaets () : SeatDto list =
        SeatRepository.GetAvailableseats()


    /// Helper: return seat counts summary (available, booked)
    /// returns (availableCount, bookedCount)
    let getSummary () : int * int =
        let seats = SeatRepository.getAllSeats()
        let folder (available, booked) (s: SeatDto) =
            match s.Status with
            | 0uy -> (available + 1, booked)
            | 1uy -> (available, booked + 1)
            | _   -> (available, booked)
        seats |> List.fold folder (0, 0)


    //// Build a seat matrix (2D array)
    let buildMatrix (seats: SeatDto list) : byte[,] =
        if List.isEmpty seats then
            Array2D.create 0 0 0uy
        else
            let maxRow = seats |> List.maxBy (fun s -> s.RowNumber) |> _.RowNumber
            let maxCol = seats |> List.maxBy (fun s -> s.ColNumber) |> _.ColNumber

            let arr = Array2D.create maxRow maxCol 0uy

            for s in seats do
                if s.RowNumber >= 1 && s.ColNumber >= 1 then
                    arr.[s.RowNumber - 1, s.ColNumber - 1] <- s.Status

            arr


    /// Render matrix in console
    let renderConsole (matrix: byte[,]) =
        let rows = matrix.GetLength(0)
        let cols = matrix.GetLength(1)

        printfn "\nCinema Seat Map:\n"

        for r in 0 .. rows - 1 do
            for c in 0 .. cols - 1 do
                match matrix.[r,c] with
                | 0uy -> printf "[ ] "
                | 1uy -> printf "[X] "
                | _   -> printf "[?] "
            printfn ""
