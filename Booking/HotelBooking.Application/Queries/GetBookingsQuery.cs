namespace HotelBooking.Application.Queries;

public sealed record GetBookingsQuery : IRequest<Response<IReadOnlyCollection<Booking>>>;