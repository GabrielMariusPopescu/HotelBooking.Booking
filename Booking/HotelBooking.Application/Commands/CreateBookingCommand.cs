namespace HotelBooking.Application.Commands;

public sealed record CreateBookingCommand(
    Guid HotelId,
    Guid UserId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests,
    IReadOnlyCollection<Guid> RoomIds) : IRequest<Response<Booking>>;