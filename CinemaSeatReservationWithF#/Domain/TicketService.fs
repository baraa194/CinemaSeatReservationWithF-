namespace CinemaSeatReservationWithFSharp.Domain

open System
open System.IO

module TicketService =

    let appendTicketToFile (ticketId: Guid) (seatId: int) (createdAt: DateTime) =
        let filePath = "tickets_log.txt"   // file in project folder
        let line = $"TicketId={ticketId} | SeatId={seatId} | CreatedAt={createdAt}\n"

        File.AppendAllText(filePath, line)

        printfn "Ticket appended to file: %s" (Path.GetFullPath(filePath))




