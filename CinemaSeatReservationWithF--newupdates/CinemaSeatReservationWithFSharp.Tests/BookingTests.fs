module BookingTests

open Xunit
open System
open CinemaSeatReservationWithFSharp.Repositories.SeatRepository

// Mock function with HallId + ScreeningId
let private mockBooking seatId hallId screeningId =
    let mutable bookedSeats = Set.empty<int * int * int> // (hallId, screeningId, seatId)
    fun seatId' hallId' screeningId' ->
        if bookedSeats.Contains((hallId', screeningId', seatId')) then AlreadyBooked
        else
            bookedSeats <- bookedSeats.Add((hallId', screeningId', seatId'))
            Booked (Guid.NewGuid())

[<Fact>]
let ``Booking a free seat should return Some GUID`` () =
    // Arrange
    let seatId = 1
    let hallId = 1
    let screeningId = 100
    let tryBookSeatMock = mockBooking seatId hallId screeningId

    // Act
    let result =
        match tryBookSeatMock seatId hallId screeningId with
        | Booked guid -> Some guid
        | _ -> None

    // Assert
    Assert.True(result.IsSome)

[<Fact>]
let ``Booking an already booked seat in same hall and screening should return None`` () =
    // Arrange
    let seatId = 2
    let hallId = 1
    let screeningId = 101
    let tryBookSeatMock = mockBooking seatId hallId screeningId

    // Book first time
    tryBookSeatMock seatId hallId screeningId |> ignore

    // Act: try booking same seat again in same hall & screening
    let result =
        match tryBookSeatMock seatId hallId screeningId with
        | Booked guid -> Some guid
        | _ -> None

    // Assert
    Assert.True(result.IsNone)

[<Fact>]
let ``Booking same seat in different hall or screening should succeed`` () =
    // Arrange
    let seatId = 3
    let hall1 = 1
    let hall2 = 2
    let screening1 = 200
    let screening2 = 201
    let tryBookSeatMock = mockBooking seatId hall1 screening1

    // Book seat in hall1 + screening1
    tryBookSeatMock seatId hall1 screening1 |> ignore

    // Act: booking same seat in hall2 + screening2
    let result1 =
        tryBookSeatMock seatId hall2 screening2 |> function
        | Booked guid -> Some guid
        | _ -> None

    // Act: booking same seat in hall1 but different screening
    let result2 =
        tryBookSeatMock seatId hall1 screening2 |> function
        | Booked guid -> Some guid
        | _ -> None

    // Assert
    Assert.True(result1.IsSome)
    Assert.True(result2.IsSome)
