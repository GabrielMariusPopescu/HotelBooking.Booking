namespace HotelBooking.Application.Repositories;

public interface IBookingRepository
{
    Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken);
    
    Task<Booking?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Booking>> GetAsync(CancellationToken cancellationToken);
    
    Task SaveChanges(CancellationToken cancellationToken);
}