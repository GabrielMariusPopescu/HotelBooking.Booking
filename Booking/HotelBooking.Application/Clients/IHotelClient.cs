namespace HotelBooking.Application.Clients;

public interface IHotelClient
{
    Task<HotelSnapshot?> GetHotel(Guid hotelId, CancellationToken cancellationToken);
}