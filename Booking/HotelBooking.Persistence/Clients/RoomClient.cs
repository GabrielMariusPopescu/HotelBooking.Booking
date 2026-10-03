namespace HotelBooking.Persistence.Clients;

public class RoomClient(HttpClient httpClient): IRoomClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    
    public async Task<IReadOnlyCollection<RoomSnapshot>?> GetRooms(Guid hotelId, CancellationToken cancellationToken)
    {
        using var responseMessage = await httpClient.GetAsync("api/rooms", cancellationToken);
        responseMessage.EnsureSuccessStatusCode();
        
        var response = await responseMessage.Content.ReadFromJsonAsync<Response<IReadOnlyCollection<RoomSnapshot>>>(JsonOptions, cancellationToken);
        return response is { IsSuccessful: true }
            ? response.Data
            : [];
    }

    public async Task<RoomReservationSnapshot> Reserve(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        using var responseMessage = await httpClient.PostAsJsonAsync("api/bookings", new
        {
            request.CheckIn, request.CheckOut, request.Guests, request.RoomIds
        }, JsonOptions, cancellationToken);
        responseMessage.EnsureSuccessStatusCode();

        var booking = await responseMessage.Content.ReadFromJsonAsync<RoomBookingData>(JsonOptions, cancellationToken)
                      ?? throw new InvalidOperationException("Room service returned an empty reservation response.");

        var rooms = booking.BookingItems
            .Select(data => 
                new ReservedRoomSnapshot(
                    data.RoomId,
            data.Room?.Name ?? string.Empty, 
                    data.Room?.RoomType ?? string.Empty, 
                    data.PricePerNight))
            .ToList();
        
        return new RoomReservationSnapshot(booking.Id, booking.TotalPrice, rooms);
    }

    public async Task CancelReservation(Guid reservationId, CancellationToken cancellationToken)
    {
        using var responseMessage = await httpClient.DeleteAsync($"api/bookings/{reservationId}", cancellationToken);
        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            return;

        responseMessage.EnsureSuccessStatusCode();
    }
}