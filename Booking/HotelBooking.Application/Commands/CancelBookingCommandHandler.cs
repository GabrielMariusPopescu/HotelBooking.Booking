namespace HotelBooking.Application.Commands;

public sealed record CancelBookingCommandHandler(IBookingRepository Repository, IRoomClient Client)
    : IRequestHandler<CancelBookingCommand, Response<Booking>>
{
    public async Task<Response<Booking>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await Repository.GetAsync(request.Id, cancellationToken);
        if (booking is null)
            return Response<Booking>.Failure($"Booking with this '{request.Id}' identifier was not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return Response<Booking>.Failure($"Booking with this '{booking.Id}' identifier is already cancelled.");
        
        if (booking.Status == BookingStatus.Completed)
            return Response<Booking>.Failure($"Booking with this '{booking.Id}' identifier is already completed and cannot be cancelled.");

        await Client.CancelReservation(booking.RoomReservationId, cancellationToken);
        booking.Cancel();

        await Repository.SaveChanges(cancellationToken);
        return Response<Booking>.Success(booking);
    }
}