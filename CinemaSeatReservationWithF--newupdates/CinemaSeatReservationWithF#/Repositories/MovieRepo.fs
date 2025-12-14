namespace CinemaSeatReservationWithFSharp.Repositories

open System
open Dapper
open CinemaSeatReservationWithFSharp.Db
open CinemaSeatReservationWithFSharp.Models

module MovieRepo =

    let getAllMovies () : Movie list =
        use conn = getConnection()
        conn.Query<Movie>("SELECT * FROM Movies") |> Seq.toList

    let getMovieById (id:int) : Movie option =
       use conn = getConnection()
       conn.Open()
       let result =
        conn.QuerySingleOrDefault<Movie>(
            "SELECT * FROM Movies WHERE Id = @Id",
            {| Id = id |}
        )
       if box result = null then None else Some result


    let createMovie (movie:Movie) : int =
        use conn = getConnection()
        let sql = """
            INSERT INTO Movies (Title, DurationMinutes, Description)
            VALUES (@Title, @DurationMinutes, @Description);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """
        conn.QuerySingle<int>(sql, movie)

    let updateMovie (movie:Movie) : unit =
        use conn = getConnection()
        let sql = """
            UPDATE Movies
            SET Title = @Title,
                DurationMinutes = @DurationMinutes,
                Description = @Description
            WHERE Id = @Id
        """
        conn.Execute(sql, movie) |> ignore

    let deleteMovie (id:int) : unit =
        use conn = getConnection()
        let sql = "DELETE FROM Movies WHERE Id = @Id"
        conn.Execute(sql, {| Id = id |}) |> ignore


