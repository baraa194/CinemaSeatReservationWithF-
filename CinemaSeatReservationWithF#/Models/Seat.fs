namespace CinemaSeatReservationWithFSharp.Models


type SeatStatus =
    | Available
    | Booked
 

type Seat = {
    SeatId: int 
    Row: int
    Col: int
    Status: SeatStatus
}


