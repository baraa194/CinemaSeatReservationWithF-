namespace CinemaSeatReservationWithFSharp.Repositories

open System
open Dapper
open CinemaSeatReservationWithFSharp.Db
open CinemaSeatReservationWithFSharp.Models

module ScreeningRepo =

    // Load all screenings
    let getAllScreenings () : Screening list =
        use conn = getConnection()
        conn.Query<Screening>("SELECT Id, MovieId, HallId, StartAt FROM Screenings") |> Seq.toList

    // Get single screening by Id
    let getScreeningById (id:int) : Screening option =
        use conn = getConnection()
        let result =
            conn.QuerySingleOrDefault<Screening>(
                "SELECT Id, MovieId, HallId, StartAt FROM Screenings WHERE Id = @Id",
                {| Id = id |})
        if box result = null then None else Some result

    // Create new screening
    let createScreening (screening:Screening) : int =
        use conn = getConnection()
        let sql = """
            INSERT INTO Screenings (MovieId, HallId, StartAt)
            VALUES (@MovieId, @HallId, @StartAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """
        conn.QuerySingle<int>(sql, screening)

    // Update existing screening
    let updateScreening (screening:Screening) : unit =
        use conn = getConnection()
        let sql = """
            UPDATE Screenings
            SET MovieId = @MovieId,
                HallId = @HallId,
                StartAt = @StartAt
            WHERE Id = @Id
        """
        conn.Execute(sql, screening) |> ignore

    // Delete a screening
    let deleteScreening (id:int) : unit =
        use conn = getConnection()
        let sql = "DELETE FROM Screenings WHERE Id = @Id"
        conn.Execute(sql, {| Id = id |}) |> ignore

    // Load screenings for a specific movie
    let getScreeningsForMovie (movieId:int) : Screening list =
        use conn = getConnection()
        let sql = "SELECT Id, MovieId, HallId, StartAt
                   FROM Screenings
                   WHERE MovieId = @MovieId
                   ORDER BY StartAt"
        conn.Query<Screening>(sql, {| MovieId = movieId |}) |> Seq.toList
