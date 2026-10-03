namespace HotelBooking.Application.Records;

public sealed record RoomReservationSnapshot(Guid Id, decimal TotalPrice, IReadOnlyCollection<ReservedRoomSnapshot> Rooms);