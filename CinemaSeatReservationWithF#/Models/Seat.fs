namespace CinemaSeatReservationWithFSharp.Models


type SeatStatus =
    | Available
    | Booked
 

type Seat = {
    SeatId: int option
    Row: int
    Col: int
    Status: SeatStatus
}


