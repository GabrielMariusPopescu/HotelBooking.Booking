namespace HotelBooking.Persistence;

public static class PersistenceServiceRegistration
{
    public static void RegisterRepositories(this IServiceCollection services)
        => services.AddTransient<IBookingRepository, BookingRepository>();
    
    public static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var assemblyName = typeof(BookingDbContext).Assembly.FullName;
            options.UseNpgsql(connectionString, builder =>
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                builder.MigrationsAssembly(assemblyName);
            });
            options.ConfigureWarnings(warnings =>
            {
                warnings.Log(CoreEventId.ManyServiceProvidersCreatedWarning);
                warnings.Log(RelationalEventId.MultipleCollectionIncludeWarning);
            });
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
    }

    public static void RegisterClients(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Services");
        services.AddHttpClient<IHotelClient, HotelClient>(client =>
        {
            var hotel = section.GetSection("Hotel");
            var url = hotel.GetValue<string>("BaseUrl");
            var address = new Uri(url ?? string.Empty);
            client.BaseAddress = address;
        });

        services.AddHttpClient<IRoomClient, RoomClient>(client =>
        {
            var room = section.GetSection("Room");
            var url = room.GetValue<string>("BaseUrl");
            var address = new Uri(url ?? string.Empty);
            client.BaseAddress = address;
        });
    }
}