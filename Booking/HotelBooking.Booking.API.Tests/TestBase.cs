namespace HotelBooking.Booking.API.Tests;

[ExcludeFromCodeCoverage]
public abstract class TestBase
{
    protected static Guid ValidHotelId => Guid.Parse("11111111-1111-1111-1111-111111111111");
    
    protected static Guid ValidUserId => Guid.Parse("22222222-2222-2222-2222-222222222222");
    
    protected static Guid ValidRoomId => Guid.Parse("33333333-3333-3333-3333-333333333333");
    
    protected static Guid ValidBookingId => Guid.Parse("44444444-4444-4444-4444-444444444444");
    
    protected static Guid ValidReservationId => Guid.Parse("55555555-5555-5555-5555-555555555555");

    protected static DateOnly CheckInDate => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
    
    protected static DateOnly CheckOutDate => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
    
    protected static int GuestCount => 2;

    private static decimal TotalPrice => 500m;

    private static string HotelName => "Test Hotel";

    private static string RoomName => "Deluxe Suite";

    private static string RoomType => "Suite";

    private static decimal PricePerNight => 250m;

    protected static HotelSnapshot CreateHotelSnapshot(
        Guid? id = null,
        string? name = null,
        bool isExcluded = false)
        => new(id ?? ValidHotelId, name ?? HotelName, isExcluded);

    protected static RoomSnapshot CreateRoomSnapshot(
        Guid? roomId = null,
        string? name = null,
        string? roomType = null,
        string roomStatus = "Available",
        decimal? pricePerNight = null,
        bool isExcluded = false)
        => new(
            roomId ?? ValidRoomId,
            name ?? RoomName,
            roomType ?? RoomType,
            roomStatus,
            pricePerNight ?? PricePerNight,
            isExcluded);

    protected static RoomReservationSnapshot CreateRoomReservationSnapshot(
        Guid? reservationId = null,
        decimal? totalPrice = null)
        => new(
            reservationId ?? ValidReservationId,
            totalPrice ?? TotalPrice,
            [new ReservedRoomSnapshot(ValidRoomId, "Luxury", "Available", PricePerNight)]);

    protected static Domain.Models.Booking CreateBooking(
        Guid? hotelId = null,
        Guid? userId = null,
        Guid? reservationId = null,
        DateOnly? checkIn = null,
        DateOnly? checkOut = null,
        int? guests = null,
        decimal? totalPrice = null,
        BookingStatus status = BookingStatus.Confirmed)
        => Domain.Models.Booking.Create(
            hotelId ?? ValidHotelId,
            userId ?? ValidUserId,
            reservationId ?? ValidReservationId,
            checkIn ?? CheckInDate,
            checkOut ?? CheckOutDate,
            guests ?? GuestCount,
            totalPrice ?? TotalPrice,
            [BookingRoom.Snapshot(ValidRoomId, RoomName, RoomType, PricePerNight)]);
}