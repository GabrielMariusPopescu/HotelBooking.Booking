namespace HotelBooking.Application.Records;

public sealed record ReservedRoomSnapshot(Guid RoomId, string RoomName, string RoomType, decimal PricePerNight);