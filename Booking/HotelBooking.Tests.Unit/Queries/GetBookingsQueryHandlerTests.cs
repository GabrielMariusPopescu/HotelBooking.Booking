namespace HotelBooking.Tests.Unit.Queries;

public class GetBookingsQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ReturnsAllBookings()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();
        var bookings = new List<Booking>
        {
            CreateBooking(),
            CreateBooking(),
            CreateBooking()
        };

        repositoryMock
            .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings.AsReadOnly());

        var handler = new GetBookingsQueryHandler(repositoryMock.Object);
        var query = new GetBookingsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WhenNoBookingsExist_ReturnsEmptyList()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();

        repositoryMock
            .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>().AsReadOnly());

        var handler = new GetBookingsQueryHandler(repositoryMock.Object);
        var query = new GetBookingsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }
}