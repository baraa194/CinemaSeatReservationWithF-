namespace CinemaSeatReservationWithFSharp.Services

open CinemaSeatReservationWithFSharp.Models
open CinemaSeatReservationWithFSharp.Repositories
open CinemaSeatReservationWithFSharp.Services 

module ScreeningService =

    let getAllScreenings () = ScreeningRepo.getAllScreenings()
    let getScreeningById id = ScreeningRepo.getScreeningById id
    let createScreening screening = ScreeningRepo.createScreening screening
    let updateScreening screening = ScreeningRepo.updateScreening screening
    let deleteScreening id = ScreeningRepo.deleteScreening id
    let getScreeningsForMovie movieId = ScreeningRepo.getScreeningsForMovie movieId

    // Get seats + hall size for a screening
    let getSeatsForScreening (screeningId:int) =
        let screening = ScreeningRepo.getScreeningById screeningId |> Option.get
        let hall = HallService.getHallById screening.HallId |> Option.get

        let rows = hall.RowsCount
        let cols = hall.ColsCount

        let seats = SeatService.getAvailableSeatsForHall hall.Id
        seats, rows, cols
