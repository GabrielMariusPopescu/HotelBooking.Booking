namespace HotelBooking.Application.Records;

public sealed record RoomBookingItemData(Guid RoomId, decimal PricePerNight, RoomReferenceData? Room);