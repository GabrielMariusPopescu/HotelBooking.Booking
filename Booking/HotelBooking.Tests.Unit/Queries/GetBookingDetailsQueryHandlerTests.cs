namespace HotelBooking.Tests.Unit.Queries;

public class GetBookingDetailsQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_WithValidBookingId_ReturnsBooking()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();
        var booking = CreateBooking();

        repositoryMock
            .Setup(x => x.GetAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var handler = new GetBookingDetailsQueryHandler(repositoryMock.Object);
        var query = new GetBookingDetailsQuery(booking.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(booking.Id);
        result.Data.HotelId.Should().Be(booking.HotelId);
        result.Data.UserId.Should().Be(booking.UserId);
    }

    [Fact]
    public async Task Handle_WithNonExistentBookingId_ReturnsFailure()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();

        repositoryMock
            .Setup(x => x.GetAsync(ValidBookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var handler = new GetBookingDetailsQueryHandler(repositoryMock.Object);
        var query = new GetBookingDetailsQuery(ValidBookingId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }
}