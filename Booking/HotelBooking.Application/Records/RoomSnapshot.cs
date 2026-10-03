namespace HotelBooking.Application.Records;

public sealed record RoomSnapshot(Guid Id, string Name, string RoomType, string RoomStatus, decimal PricePerNight, bool IsExcluded);