namespace CinemaSeatReservationWithFSharp.Services

open System
open CinemaSeatReservationWithFSharp.Repositories
open CinemaSeatReservationWithFSharp.Models



module HallService =

    
    let getAllHalls () : Hall list =
        HallsRepo.getAllHalls()


    let getHallById (id:int) : Hall option =
        HallsRepo.getHallById id


    let createHall (hall:Hall) : Result<int, string> =
        match HallsRepo.getAllHalls() |> List.tryFind (fun h -> h.Name = hall.Name) with
        | Some _ -> Error $"Hall with name '{hall.Name}' already exists."
        | None -> Ok (HallsRepo.createHall hall)


    let updateHall (hall:Hall) : Result<unit, string> =
        match HallsRepo.getAllHalls() |> List.tryFind (fun h -> h.Id <> hall.Id && h.Name = hall.Name) with
        | Some _ -> Error $"Another hall with name '{hall.Name}' already exists."
        | None ->
            HallsRepo.updateHall hall
            Ok ()

    let deleteHall (id:int) : unit =
        HallsRepo.deleteHall id



   

