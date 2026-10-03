namespace HotelBooking.Application.Commands;

public sealed record CancelBookingCommand(Guid Id) : IRequest<Response<Booking>>;