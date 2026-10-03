namespace HotelBooking.Application;

public static class ApplicationServicesRegistration
{
    public static void RegisterMediator(this IServiceCollection services)
    {
        var assembly = typeof(CreateBookingCommandValidator).Assembly;
        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
    }
}