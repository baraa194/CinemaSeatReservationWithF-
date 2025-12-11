module CinemaSeatReservationWithFSharp.Services.UserService

open System
open CinemaSeatReservationWithFSharp.Models
open CinemaSeatReservationWithFSharp.Repositories.UserRepo


let registerUser (username:string) (email:string) (password:string) : Result<int,string> =
  
    if String.IsNullOrWhiteSpace username then
        Error "Username must not be empty."
    elif String.IsNullOrWhiteSpace email then
         Error "Email must not be empty "
    elif String.IsNullOrWhiteSpace password then
        Error "Password must not be empty."
    else
        // check for existing user
        match getUserByUsername username with
        | Some _ -> Error $"Username '%s{username}' is already taken."
        | None ->
            try
                let newId = createUser username email password
                Ok newId
            with ex ->
               
                Error $"Failed to create user: %s{ex.Message}"


/// Returns AuthResult = Success User | InvalidCredentials | NotFound
let loginUser (username:string) (password:string) : AuthResult =
    login username password



