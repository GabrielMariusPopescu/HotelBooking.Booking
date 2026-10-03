namespace HotelBooking.Application.Records;

public sealed record RoomBookingData(Guid Id, decimal TotalPrice, List<RoomBookingItemData> BookingItems);