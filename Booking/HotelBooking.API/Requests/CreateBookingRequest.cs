namespace HotelBooking.API.Requests;

public sealed record CreateBookingRequest(
    Guid HotelId,
    Guid UserId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests,
    IReadOnlyCollection<Guid> RoomIds);