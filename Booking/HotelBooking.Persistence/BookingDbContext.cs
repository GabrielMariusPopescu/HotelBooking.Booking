namespace HotelBooking.Persistence;

public sealed class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();
    
    public DbSet<BookingRoom> BookingRooms => Set<BookingRoom>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(booking => booking.Id);
            entity.Property(booking => booking.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(booking => booking.TotalPrice)
                .HasPrecision(18, 2);
            entity.HasIndex(booking => booking.UserId);
            entity.HasIndex(booking => booking.HotelId);
            entity.HasMany(booking => booking.Rooms)
                .WithOne()
                .HasForeignKey(room => room.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookingRoom>(entity =>
        {
            entity.HasKey(room => room.Id);
            entity.Property(room => room.PricePerNight).HasPrecision(18, 2);
            entity.Property(room => room.RoomName).HasMaxLength(100).IsRequired();
            entity.Property(room => room.RoomType).HasMaxLength(50).IsRequired();
        });
    }
}