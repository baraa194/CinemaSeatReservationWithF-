namespace CinemaSeatReservationWithFSharp.Services

open System
open CinemaSeatReservationWithFSharp.Repositories
open CinemaSeatReservationWithFSharp.Repositories.SeatRepository

module SeatService =

    /// Load all seats for default hall (convenience)
    let loadAllSeats () : SeatDto list =
        // default hallId = 1 (you can change or create a wrapper that accepts hallId)
        SeatRepository.getAllSeatsForHall 1

    /// Get all seats for a given hall (booked + available)
    let getAllSeatsForHall (hallId:int) : SeatDto list =
        SeatRepository.getAllSeatsForHall hallId

    /// Load available seats for a hall
    let getAvailableSeatsForHall (hallId:int) : SeatDto list =
        SeatRepository.getAvailableSeats hallId

    /// Try book seat for screening -> returns ticket id if succeeded
    let tryBookSeat (seatId:int) (screeningId:int) : Guid option =
        match SeatRepository.tryBookSeatForScreening seatId screeningId with
        | BookingResult.Booked guid -> Some guid
        | _ -> None

    /// Build 2D matrix for a hall (uses ALL seats so matrix shows booked X and available O)
    let buildMatrixForHall (hallId:int) (rows:int) (cols:int) : byte[,] =
        SeatRepository.buildMatrixForHall hallId rows cols

    /// Render seat matrix in console for a hall
    let renderMatrix (hallId:int) (rows:int) (cols:int) =
        let matrix = buildMatrixForHall hallId rows cols
        SeatRepository.renderConsole matrix

    /// Sanitize seats (set invalid status to 0)
    let sanitizeSeats () =
        SeatRepository.sanitizeStatuses ()
