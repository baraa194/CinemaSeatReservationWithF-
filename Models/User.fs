namespace CinemaSeatReservationWithFSharp.Models

open System
[<CLIMutable>]

type User = {
    Id:int
    Username:string
    Email: string
    PasswordHash: byte[]
    PasswordSalt: byte[]
    RoleId: int
    CreatedAt: DateTime
}
