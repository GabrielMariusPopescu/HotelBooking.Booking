namespace HotelBooking.Application.Queries;

public sealed record GetBookingDetailsQuery(Guid Id) : IRequest<Response<Booking>>;
