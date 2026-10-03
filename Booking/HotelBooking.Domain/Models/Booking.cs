namespace HotelBooking.Domain.Models;

public sealed class Booking
{
    private Booking() { }

    public Guid Id { get; private set; }

    public Guid HotelId { get; private set; }

    public Guid UserId { get; private set; }

    public Guid RoomReservationId { get; private set; }

    public DateOnly CheckIn { get; private set; }

    public DateOnly CheckOut { get; private set; }

    public int Guests { get; private set; }

    public decimal TotalPrice { get; private set; }

    public BookingStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public List<BookingRoom> Rooms { get; } = [];

    public static Booking Create(Guid hotelId, Guid userId, Guid roomReservationId,
        DateOnly checkIn, DateOnly checkOut, int guests, decimal totalPrice,
        IEnumerable<BookingRoom> rooms)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            UserId = userId,
            RoomReservationId = roomReservationId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Guests = guests,
            TotalPrice = totalPrice,
            Status = BookingStatus.Confirmed,
            CreatedAtUtc = DateTime.UtcNow
        };

        booking.Rooms.AddRange(rooms);
        return booking;
    }
    
    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return;

        if (Status == BookingStatus.Completed)
            throw new InvalidOperationException("A completed booking cannot be cancelled.");

        Status = BookingStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}