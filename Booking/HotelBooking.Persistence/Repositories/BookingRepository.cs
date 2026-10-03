namespace HotelBooking.Persistence.Repositories;

public class BookingRepository(BookingDbContext context): IBookingRepository
{
    public async Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        await context.Bookings.AddAsync(booking, cancellationToken);
        await SaveChanges(cancellationToken);
        return booking;
    }

    public async Task<Booking?> GetAsync(Guid id, CancellationToken cancellationToken) 
        => await context.Bookings
            .Include(booking => booking.Rooms)
            .SingleOrDefaultAsync(booking => booking.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Booking>> GetAsync(CancellationToken cancellationToken) 
        => await context.Bookings
            .Include(booking => booking.Rooms)
            .OrderByDescending(booking => booking.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task SaveChanges(CancellationToken cancellationToken) 
        => await context.SaveChangesAsync(cancellationToken);
}