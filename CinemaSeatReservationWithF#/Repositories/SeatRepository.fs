namespace CinemaSeatReservationWithFSharp.Repositories

open System
open System.Data
open Dapper
open Microsoft.Data.SqlClient
open CinemaSeatReservationWithFSharp

module SeatRepository =


    let private statusAvailable : byte = 0uy
    let private statusBooked    : byte = 1uy



    // Booking result 
    type BookingResult =
        | Booked of Guid       
        | AlreadyBooked
        | NotFound
        | Error of string

    //connection helper
    let private getConnection() : IDbConnection =
        Db.getConnection()

    // read all seats
    let getAllSeats() : SeatDto list =
        use conn = getConnection()
        let sql = "SELECT SeatId, RowNumber, ColNumber, Status FROM Seats ORDER BY RowNumber, ColNumber"
        conn.Query<SeatDto>(sql) |> Seq.toList

    // get single seat by id (optional)
    let tryGetSeatById (seatId:int) : SeatDto option =
        use conn = getConnection()
        let sql = "SELECT SeatId, RowNumber, ColNumber, Status FROM Seats WHERE SeatId = @Id"
        let dto = conn.QuerySingleOrDefault<SeatDto>(sql, {| Id = seatId |})
        if isNull (box dto) then None else Some dto

    //get available seats 
    let GetAvailableseats() : SeatDto list =
        use conn = getConnection()
        let sql = "SELECT SeatId, RowNumber, ColNumber, Status FROM Seats WHERE Status = @Status ORDER BY RowNumber, ColNumber"
        conn.Query<SeatDto>(sql, {| Status = statusAvailable |}) |> Seq.toList

    // book seats
    let tryBookSeatAndCreateTicketResult (seatId:int) : BookingResult =
        use conn = getConnection()
        conn.Open()
        use tran = conn.BeginTransaction()
        try
            let updateSql = "UPDATE Seats SET 
            Status = @Booked WHERE SeatId = @Id AND Status = @Available"
            let rows =
                conn.Execute(updateSql,
                             {| Booked = statusBooked; Id = seatId; Available = statusAvailable |},
                             transaction = tran)

            if rows = 0 then
                let existsSql = "SELECT COUNT(1) FROM Seats WHERE SeatId = @Id"
                let exists = conn.ExecuteScalar<int>(existsSql, {| Id = seatId |}, transaction = tran)
                tran.Rollback()
                if exists = 0 then NotFound else AlreadyBooked
            else
                let ticketId = Guid.NewGuid()
                let insertSql = "INSERT INTO Tickets (TicketId, SeatId, CreatedAt) 
                VALUES (@TicketId, @SeatId, @CreatedAt)"
                conn.Execute(insertSql,
                             {| TicketId = ticketId; SeatId = seatId; CreatedAt= DateTime.UtcNow |},
                             transaction = tran) |> ignore

                tran.Commit()
                Booked ticketId
        with ex ->
            try tran.Rollback() with _ -> ()
            Error ex.Message

    // book + ticket
    let tryBookSeatAndCreateTicket (seatId:int) : Guid option =
        use conn = getConnection()
        conn.Open()
        use tran = conn.BeginTransaction()
        try
            let updateSql = "UPDATE Seats SET Status = @Booked WHERE SeatId = @Id AND Status = @Available"
            let rows =
                conn.Execute(updateSql,
                             {| Booked = statusBooked; Id = seatId; Available = statusAvailable |},
                             transaction = tran)
            if rows <= 0 then
                tran.Rollback()
                None
            else
                let ticketId = Guid.NewGuid()
                let insertSql = "INSERT INTO Tickets (TicketId, SeatId, CreatedAt) VALUES (@TicketId, @SeatId, @CreatedAt)"
                conn.Execute(insertSql,
                             {| TicketId = ticketId; SeatId = seatId; CreatedAt = DateTime.UtcNow |},
                             transaction = tran) |> ignore
                tran.Commit()
                Some ticketId
        with
        | ex ->
            try tran.Rollback() with _ -> ()
            raise ex

    //sanitize
    let sanitizeStatuses () =
        use conn = getConnection()
        let sql = "UPDATE dbo.Seats SET Status = @Available WHERE Status NOT IN (0,1)"
        conn.Execute(sql, {| Available = statusAvailable |}) |> ignore
