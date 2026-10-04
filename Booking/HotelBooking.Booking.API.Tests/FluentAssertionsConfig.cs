[assembly: FluentAssertions.Extensibility.AssertionEngineInitializer(
    typeof(FluentAssertionsConfig),
    nameof(FluentAssertionsConfig.AcknowledgeSoftWarning))]

namespace HotelBooking.Booking.API.Tests;

[ExcludeFromCodeCoverage]
public static class FluentAssertionsConfig
{
    public static void AcknowledgeSoftWarning()
    {
        License.Accepted = true;
    }
}