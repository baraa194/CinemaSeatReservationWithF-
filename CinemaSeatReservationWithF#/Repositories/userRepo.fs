module CinemaSeatReservationWithFSharp.Repositories.UserRepo

open System
open System.Data
open Dapper
open CinemaSeatReservationWithFSharp.Models
open System.IO
open CinemaSeatReservationWithFSharp.Db
open CinemaSeatReservationWithFSharp.Password
open CinemaSeatReservationWithFSharp

/// Helper to run an action with an open connection
let private withConn (f: IDbConnection -> 'T) =
    use conn = getConnection()
    conn.Open()
    f conn

/// Create user (returns new user id)
let createUser (username:string) (email:string) (password:string) : int =
    let (hash, salt) = Password.hashPassword password
    withConn (fun conn ->
        let sql = """
            INSERT INTO Users (Username, Email ,PasswordHash, PasswordSalt, RoleId)
            VALUES (@Username,@Email, @Hash, @Salt, (SELECT Id FROM Roles WHERE Name = 'User'));
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        """
      
        conn.QuerySingle<int>(sql, {| Username = username;   Email = email ;Hash = hash; Salt = salt |})
    )

/// Get user by username 
let getUserByUsername (username:string) : User option =
    withConn (fun conn ->
        let sql = """
            SELECT u.Id, u.Username, u.PasswordHash, u.PasswordSalt, u.RoleId, r.Name AS RoleName, u.CreatedAt
            FROM Users u
            JOIN Roles r ON u.RoleId = r.Id
            WHERE u.Username = @Username
        """
        let user = conn.QuerySingleOrDefault<User>(sql, {| Username = username |})
        if isNull (box user) then None else Some user
    )

/// Verify credentials (login)
type AuthResult =
    | Success of User
    | InvalidCredentials
    | NotFound

let login (username:string) (password:string) : AuthResult =
    match getUserByUsername username with
    | None -> NotFound
    | Some user ->
        if Password.verifyPassword password user.PasswordHash user.PasswordSalt then
            Success user
        else
            InvalidCredentials


