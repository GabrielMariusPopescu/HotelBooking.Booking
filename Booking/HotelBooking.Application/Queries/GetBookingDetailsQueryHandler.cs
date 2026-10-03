namespace HotelBooking.Application.Queries;

public record GetBookingDetailsQueryHandler(IBookingRepository Repository)
    : IRequestHandler<GetBookingDetailsQuery, Response<Booking>>
{
    public async Task<Response<Booking>> Handle(GetBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        var booking = await Repository.GetAsync(request.Id, cancellationToken);
        return booking != null
            ? Response<Booking>.Success(booking)
            : Response<Booking>.Failure($"Booking with this '{request.Id}' identifier was not found.");
    }
}