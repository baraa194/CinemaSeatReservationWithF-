namespace CinemaSeatReservationWithFSharp.Domain

open CinemaSeatReservationWithFSharp.Models

module SeatLayout =

   // create matrix
   let createMatrix (rows:int) (cols:int) :SeatStatus[,]=
    Array2D.create rows cols SeatStatus.Available

// validate indexes
   let private isValidIndex (rows:int) (cols:int) (r:int) (c:int) =
        r >= 0 && r < rows && c >= 0 && c < cols

// get seatstatus

   let getSeatStatus (matrix:SeatStatus[,]) (r:int) (c:int)=
     let rows=Array2D.length1 matrix
     let cols=Array2D.length2 matrix
     if isValidIndex rows cols r c then Some matrix.[r,c] else None




