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


    let private getConnection() : IDbConnection =
        Db.getConnection()


    let tryGetSeatById (seatId:int) : SeatDto option =
        use conn = getConnection()
        conn.Open()

        let sql =
            "SELECT SeatId, ScreeningId, RowNumber, ColNumber, Status 
             FROM Seats 
             WHERE SeatId = @Id"

        let dto =
            conn.QuerySingleOrDefault<SeatDto>(
                sql,
                {| Id = seatId |}
            )

        if box dto = null then None else Some dto


    let getAllSeatsForScreening (screeningId:int) : SeatDto list =
        use conn = getConnection()

        let sql =
            "SELECT SeatId, ScreeningId, RowNumber, ColNumber, Status
             FROM Seats 
             WHERE ScreeningId = @ScreeningId
             ORDER BY RowNumber, ColNumber"

        conn.Query<SeatDto>(sql, {| ScreeningId = screeningId |})
        |> Seq.toList


 
    let getAvailableSeatsForScreening (screeningId:int) : SeatDto list =
        use conn = getConnection()

        let sql =
            "SELECT SeatId, ScreeningId, RowNumber, ColNumber, Status
             FROM Seats 
             WHERE Status = @Status AND ScreeningId = @ScreeningId
             ORDER BY RowNumber, ColNumber"

        conn.Query<SeatDto>(
            sql,
            {| Status = statusAvailable
               ScreeningId = screeningId |}
        )
        |> Seq.toList



    let tryBookSeatForScreening (seatId:int) (screeningId:int) : BookingResult =
        use conn = getConnection()
        conn.Open()

        use tran = conn.BeginTransaction()

        try
            let exists =
                conn.ExecuteScalar<int>(
                    "SELECT COUNT(1) FROM Seats WHERE SeatId = @SeatId AND ScreeningId = @ScreeningId",
                    {| SeatId = seatId; ScreeningId = screeningId |},
                    transaction = tran
                )

            if exists = 0 then
                tran.Rollback()
                NotFound
            else
                let rowsUpdated =
                    conn.Execute(
                        "UPDATE Seats SET Status = @Booked WHERE SeatId = @SeatId AND ScreeningId = @ScreeningId AND Status = @Available",
                        {| Booked = statusBooked; SeatId = seatId; ScreeningId = screeningId; Available = statusAvailable |},
                        transaction = tran
                    )

                if rowsUpdated = 0 then
                    tran.Rollback()
                    AlreadyBooked
                else
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


   
    let sanitizeStatuses () =
        use conn = getConnection()

        let sql =
            "UPDATE Seats 
             SET Status = @Available 
             WHERE Status NOT IN (0, 1)"

        conn.Execute(sql, {| Available = statusAvailable |}) |> ignore


    
    let buildMatrix (seats: SeatDto list) (rows:int) (cols:int) : byte[,] =
        let matrix = Array2D.create rows cols statusAvailable

        seats
        |> List.iter (fun s ->
            let r = s.RowNumber - 1
            let c = s.ColNumber - 1
            if r >= 0 && r < rows && c >= 0 && c < cols then
                matrix.[r, c] <- s.Status
        )

        matrix


 
    type HallSize = { RowsCount: int; ColsCount: int }

    let buildMatrixForScreening (screeningId:int) : byte[,] =
        use conn = getConnection()

        let hall =
            conn.QuerySingleOrDefault<HallSize>(
                "SELECT h.RowsCount, h.ColsCount
                 FROM Halls h
                 INNER JOIN Screenings s ON s.HallId = h.Id
                 WHERE s.Id = @ScreeningId",
                {| ScreeningId = screeningId |}
            )

        if isNull (box hall) then
            Array2D.create 0 0 statusAvailable
        else
            let seats = getAllSeatsForScreening screeningId
            buildMatrix seats hall.RowsCount hall.ColsCount



    let renderConsole (matrix: byte[,]) =
        let rows = matrix.GetLength(0)
        let cols = matrix.GetLength(1)

        for r in 0 .. rows - 1 do
            for c in 0 .. cols - 1 do
                let ch =
                    match matrix.[r, c] with
                    | 0uy -> 'O'   
                    | 1uy -> 'X'   
                    | _   -> '?'

                printf "%c " ch
            printfn ""


   
    let renderMatrixForScreening (screeningId:int) =
        let matrix = buildMatrixForScreening screeningId
        renderConsole matrix
