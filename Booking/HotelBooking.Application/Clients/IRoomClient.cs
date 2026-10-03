namespace HotelBooking.Application.Clients;

public interface IRoomClient
{
    Task<IReadOnlyCollection<RoomSnapshot>?> GetRooms(Guid hotelId, CancellationToken cancellationToken);

    Task<RoomReservationSnapshot> Reserve(CreateBookingCommand request, CancellationToken cancellationToken);
    
    Task CancelReservation(Guid reservationId, CancellationToken cancellationToken);
}