namespace HotelBooking.Application.Queries;

public record GetBookingsQueryHandler(IBookingRepository Repository)
    : IRequestHandler<GetBookingsQuery, Response<IReadOnlyCollection<Booking>>>
{
    public async Task<Response<IReadOnlyCollection<Booking>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await Repository.GetAsync(cancellationToken);
        return bookings.Any()
            ? Response<IReadOnlyCollection<Booking>>.Success(bookings)
            : Response<IReadOnlyCollection<Booking>>.Failure("No bookings found.");
    }
}