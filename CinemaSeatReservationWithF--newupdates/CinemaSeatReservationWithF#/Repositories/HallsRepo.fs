namespace CinemaSeatReservationWithFSharp.Repositories

open System
open Dapper
open CinemaSeatReservationWithFSharp.Models
open CinemaSeatReservationWithFSharp.Db

module HallsRepo=
//get all halls
 let getAllHalls () : Hall list =
        use conn = getConnection()
        conn.Open()
        conn.Query<Hall>("SELECT * FROM Halls") |> Seq.toList

//get hall by id
 let getHallById (id:int) : Hall option =
    use conn = getConnection()
    conn.Open()
    let result =
        conn.QuerySingleOrDefault<Hall>(
            "SELECT * FROM Halls WHERE Id=@Id",
            {| Id = id |}
        )
    if box result = null then None else Some result

// create new hall
 let createHall (hall:Hall) : int =
        use conn = getConnection()
        conn.Open()
        let sql = """
            INSERT INTO Halls (Name, RowsCount, ColsCount)
            VALUES (@Name, @RowsCount, @ColsCount);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """
        conn.QuerySingle<int>(sql, hall)

 // update hall
 let updateHall (hall:Hall) : unit =
        use conn = getConnection()
        conn.Open()
        let sql = """
            UPDATE Halls
            SET Name=@Name, RowsCount=@RowsCount, ColsCount=@ColsCount
            WHERE Id=@Id
        """
        conn.Execute(sql, hall) |> ignore

//delete hall
 let deleteHall (id:int) : unit =
        use conn = getConnection()
        conn.Open()
        conn.Execute("DELETE FROM Halls WHERE Id=@Id", {| Id = id |}) |> ignore

