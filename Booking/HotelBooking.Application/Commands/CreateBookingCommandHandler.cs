namespace HotelBooking.Application.Commands;

public sealed class CreateBookingCommandHandler(
    IHotelClient hotelClient,
    IRoomClient roomClient,
IBookingRepository repository) : IRequestHandler<CreateBookingCommand, Response<Booking>>
{
    public async Task<Response<Booking>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (request.HotelId == Guid.Empty ||
            request.UserId == Guid.Empty)
            return Response<Booking>.Failure($"{request.HotelId} and {request.UserId} are required.");

        if (request.CheckIn < DateOnly.FromDateTime(DateTime.UtcNow) ||
            request.CheckOut <= request.CheckIn)
            return Response<Booking>.Failure(
                $"{request.CheckOut} must be after {request.CheckIn} and the stay must start today or later.");

        if (request.Guests < 1 ||
            request.RoomIds.Count == 0 ||
            request.RoomIds.Distinct().Count() != request.RoomIds.Count)
            return Response<Booking>.Failure("At least one unique room and one guest are required.");

        var hotel = await hotelClient.GetHotel(request.HotelId, cancellationToken);
        if (hotel is null ||
            hotel.IsExcluded)
            return Response<Booking>.Failure("The requested hotel was not found or is excluded.");

        var hotelRooms = await roomClient.GetRooms(hotel.Id, cancellationToken);
        if (hotelRooms is null || !hotelRooms.Any())
            return Response<Booking>.Failure($"No rooms found for hotel with the '{hotel.Id}' identifier.");
        
        var selectedRooms = hotelRooms.Where(hotelRoom => request.RoomIds.Contains(hotelRoom.Id)).ToList();
        if (selectedRooms.Count != request.RoomIds.Count)
            return Response<Booking>.Failure("One or more requested rooms do not exist.");

        if (selectedRooms.Any(selectedRoom => selectedRoom.IsExcluded ||
                                              !string.Equals(selectedRoom.RoomStatus, "Available",
                                                  StringComparison.OrdinalIgnoreCase)))
            return Response<Booking>.Failure("One or more requested room are not available for booking.");

        var reservation = await roomClient.Reserve(request, cancellationToken);
        var dictionary = selectedRooms.ToDictionary(selectedRoom => selectedRoom.Id);
        var bookingRooms = reservation.Rooms.Select(reservedRoom =>
        {
            var snapshot = dictionary[reservedRoom.RoomId];
            return BookingRoom.Snapshot(reservedRoom.RoomId, snapshot.Name, snapshot.RoomType, snapshot.PricePerNight);
        });

        var booking = Booking.Create(
            request.HotelId, 
            request.UserId, 
            reservation.Id, 
            request.CheckIn, 
            request.CheckOut,
            request.Guests, 
            reservation.TotalPrice, 
            bookingRooms);

        try
        {
            var created = await repository.AddAsync(booking, cancellationToken);
            return Response<Booking>.Success(created);
        }
        catch
        {
            await roomClient.CancelReservation(reservation.Id, CancellationToken.None);
            throw;
        }
    }
}