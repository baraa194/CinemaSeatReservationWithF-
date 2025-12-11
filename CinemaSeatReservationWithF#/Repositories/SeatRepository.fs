namespace CinemaSeatReservationWithFSharp.Repositories

open System
open System.Data
open Dapper
open Microsoft.Data.SqlClient
open CinemaSeatReservationWithFSharp
open CinemaSeatReservationWithFSharp.Models

module SeatRepository =

    let private statusAvailable : byte = 0uy
    let private statusBooked    : byte = 1uy

    type BookingResult =
        | Booked of Guid
        | AlreadyBooked
        | NotFound
        | Error of string

    // -----------------------
    // Connection helper
    // -----------------------
    let private getConnection() : IDbConnection =
        Db.getConnection()


    // ------------------------------
    // Get single seat by Id
    // ------------------------------
    let tryGetSeatById (seatId:int) : SeatDto option =
        use conn = getConnection()
        conn.Open()

        let sql =
            "SELECT SeatId, HallId, RowNumber, ColNumber, Status 
             FROM Seats 
             WHERE SeatId = @Id"

        let dto =
            conn.QuerySingleOrDefault<SeatDto>(
                sql,
                {| Id = seatId |}
            )

        if box dto = null then None else Some dto


    // -----------------------------------------------------
    // Get all seats for a hall (includes booked + available)
    // -----------------------------------------------------
    let getAllSeatsForHall (hallId:int) : SeatDto list =
        use conn = getConnection()

        let sql =
            "SELECT SeatId, HallId, RowNumber, ColNumber, Status
             FROM Seats 
             WHERE HallId = @HallId
             ORDER BY RowNumber, ColNumber"

        conn.Query<SeatDto>(sql, {| HallId = hallId |})
        |> Seq.toList


    // -----------------------------------------------------
    // Get available seats (filtered by hallId) - kept for convenience
    // -----------------------------------------------------
    let getAvailableSeats (hallId:int) : SeatDto list =
        use conn = getConnection()

        let sql =
            "SELECT SeatId, HallId, RowNumber, ColNumber, Status
             FROM Seats 
             WHERE Status = @Status AND HallId = @HallId
             ORDER BY RowNumber, ColNumber"

        conn.Query<SeatDto>(
            sql,
            {| Status = statusAvailable
               HallId = hallId |}
        )
        |> Seq.toList


    // -----------------------------
    // Book seat for a screening (atomic)
    // -----------------------------
    let tryBookSeatForScreening (seatId:int) (screeningId:int) : BookingResult =
        use conn = getConnection()
        conn.Open()

        use tran = conn.BeginTransaction()

        try
            // 1) ensure seat exists
            let exists =
                conn.ExecuteScalar<int>(
                    "SELECT COUNT(1) FROM Seats WHERE SeatId = @SeatId",
                    {| SeatId = seatId |},
                    transaction = tran
                )

            if exists = 0 then
                tran.Rollback()
                NotFound
            else
                // 2) Update seat status (only if available)
                let rowsUpdated =
                    conn.Execute(
                        "UPDATE Seats SET Status = @Booked WHERE SeatId = @SeatId AND Status = @Available",
                        {| Booked = statusBooked; SeatId = seatId; Available = statusAvailable |},
                        transaction = tran
                    )

                if rowsUpdated = 0 then
                    // seat either already booked
                    tran.Rollback()
                    AlreadyBooked
                else
                    // 3) Insert ticket
                    let ticketId = Guid.NewGuid()
                    conn.Execute(
                        "INSERT INTO Tickets (TicketId, SeatId, ScreeningId, CreatedAt) 
                         VALUES (@TicketId, @SeatId, @ScreeningId, @CreatedAt)",
                        {| TicketId = ticketId; SeatId = seatId; ScreeningId = screeningId; CreatedAt = DateTime.UtcNow |},
                        transaction = tran
                    ) |> ignore

                    tran.Commit()
                    Booked ticketId

        with ex ->
            try tran.Rollback() with _ -> ()
            Error ex.Message


    // -----------------------
    // Sanitize seat statuses
    // -----------------------
    let sanitizeStatuses () =
        use conn = getConnection()

        let sql =
            "UPDATE Seats 
             SET Status = @Available 
             WHERE Status NOT IN (0, 1)"

        conn.Execute(sql, {| Available = statusAvailable |}) |> ignore


    // ----------------------------
    // Build 2D matrix from seats list
    // ----------------------------
    let buildMatrix (seats: SeatDto list) (rows:int) (cols:int) : byte[,] =
        let matrix = Array2D.create rows cols statusAvailable

        seats
        |> List.iter (fun s ->
            // guard in case DB has row/col outside expected range
            let r = s.RowNumber - 1
            let c = s.ColNumber - 1
            if r >= 0 && r < rows && c >= 0 && c < cols then
                matrix.[r, c] <- s.Status
        )

        matrix


    // ----------------------------
    // Build matrix for a hall (uses all seats)
    // ----------------------------
    let buildMatrixForHall (hallId:int) (rows:int) (cols:int) : byte[,] =
        let seats = getAllSeatsForHall hallId
        buildMatrix seats rows cols


    // ----------------------------
    // Render matrix visually in the console
    // ----------------------------
    let renderConsole (matrix: byte[,]) =
        let rows = matrix.GetLength(0)
        let cols = matrix.GetLength(1)

        for r in 0 .. rows - 1 do
            for c in 0 .. cols - 1 do
                let ch =
                    match matrix.[r, c] with
                    | 0uy -> 'O'   // available
                    | 1uy -> 'X'   // booked
                    | _   -> '?'

                printf "%c " ch
            printfn ""


    // ----------------------------
    // Helper: build + render for a hall (convenience)
    // ----------------------------
    let renderMatrixForHall (hallId:int) (rows:int) (cols:int) =
        let matrix = buildMatrixForHall hallId rows cols
        renderConsole matrix
