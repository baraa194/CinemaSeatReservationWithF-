namespace CinemaSeatReservationWithFSharp.Services

open System
open CinemaSeatReservationWithFSharp.Repositories
open CinemaSeatReservationWithFSharp.Repositories.SeatRepository

module SeatService =

  
    // Load all seats (raw list)
  
    let loadAllSeats () : SeatDto list =
        SeatRepository.getAvailableSeats 1  


   
    // Load available seats for a hall
   
    let getAvailableSeatsForHall (hallId:int) : SeatDto list =
        SeatRepository.getAvailableSeats hallId


   
    // Try book seat for screening
  
    let tryBookSeat (seatId:int) (screeningId:int) : Guid option =
        match SeatRepository.tryBookSeatForScreening seatId screeningId with
        | BookingResult.Booked guid -> Some guid
        | _ -> None


   
    // Build 2D matrix for a hall
  
    let buildMatrixForHall (hallId:int) (rows:int) (cols:int) : byte[,] =
        let seats = SeatRepository.getAvailableSeats hallId
        SeatRepository.buildMatrix seats rows cols


   
    // Render seat matrix in console
 
    let renderMatrix (hallId:int) (rows:int) (cols:int) =
        let matrix = buildMatrixForHall hallId rows cols
        SeatRepository.renderConsole matrix



  
    // Sanitize seats (set invalid status to 0)
 
    let sanitizeSeats () =
        SeatRepository.sanitizeStatuses ()
