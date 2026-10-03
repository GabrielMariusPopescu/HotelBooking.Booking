namespace HotelBooking.Domain.Models;

public sealed class BookingRoom
{
    private BookingRoom() { }

    public BookingRoom(Guid bookingId) => BookingId = bookingId;

    public Guid Id { get; private set; }

    public Guid BookingId { get; private set; }

    public Guid RoomId { get; private set; }

    public string RoomName { get; private set; } = string.Empty;
    
    public string RoomType { get; private set; } = string.Empty;

    public decimal PricePerNight { get; private set; }

    public static BookingRoom Snapshot(
        Guid roomId,
        string roomName,
        string roomType,
        decimal pricePerNight)
        => new()
        {
            Id = Guid.NewGuid(),
RoomId = roomId,
RoomName = roomName,
RoomType = roomType,
PricePerNight = pricePerNight
        };
}