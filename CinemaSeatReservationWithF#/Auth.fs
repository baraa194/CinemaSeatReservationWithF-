namespace CinemaSeatReservationWithFSharp


open CinemaSeatReservationWithFSharp.Models


module Auth =


   let getRoleName (roleId:int) =
    match roleId with
    | 1 -> "Admin"
    | 2 -> "User"
    | _ -> "Unknown"

   let authorizeRoles (userOpt: User option) (allowedRoles: string list) : Result<unit,string> =
    match userOpt with
    | None -> Error "Not authenticated"
    | Some user ->
        let roleName = getRoleName user.RoleId
        if List.exists ((=) roleName) allowedRoles then Ok ()
        else Error $"User '{user.Username}' does not have required role."

