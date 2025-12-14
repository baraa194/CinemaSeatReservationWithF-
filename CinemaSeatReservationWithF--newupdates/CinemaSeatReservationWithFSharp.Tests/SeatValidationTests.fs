module SeatValidationTests

open Xunit
open CinemaSeatReservationWithFSharp.Services

// Validation function for seat coordinates within a hall
let isValidSeat (row:int) (col:int) (hallRows:int) (hallCols:int) =
    row >= 0 && row < hallRows && col >= 0 && col < hallCols

[<Fact>]
let ``Valid seat coordinates should return true`` () =
    let hallRows = 5
    let hallCols = 5
    let result = isValidSeat 2 3 hallRows hallCols
    Assert.True(result)

[<Theory>]
[<InlineData(-1,0)>]
[<InlineData(0,-1)>]
[<InlineData(5,2)>]
[<InlineData(2,5)>]
let ``Invalid seat coordinates should return false`` (row:int, col:int) =
    let hallRows = 5
    let hallCols = 5
    let result = isValidSeat row col hallRows hallCols
    Assert.False(result)


