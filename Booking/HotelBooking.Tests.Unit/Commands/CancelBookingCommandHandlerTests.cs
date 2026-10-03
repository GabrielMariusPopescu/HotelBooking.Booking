namespace HotelBooking.Tests.Unit.Commands;

public class CancelBookingCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_WithValidBooking_CancelsSuccessfully()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();
        var clientMock = new Mock<IRoomClient>();

        var booking = CreateBooking(status: BookingStatus.Confirmed);

        repositoryMock
            .Setup(x => x.GetAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        clientMock
            .Setup(x => x.CancelReservation(booking.RoomReservationId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        repositoryMock
            .Setup(x => x.SaveChanges(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CancelBookingCommandHandler(repositoryMock.Object, clientMock.Object);
        var command = new CancelBookingCommand(booking.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(BookingStatus.Cancelled);
        clientMock.Verify(x => x.CancelReservation(booking.RoomReservationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentBooking_ReturnsFailure()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();
        var clientMock = new Mock<IRoomClient>();

        repositoryMock
            .Setup(x => x.GetAsync(ValidBookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var handler = new CancelBookingCommandHandler(repositoryMock.Object, clientMock.Object);
        var command = new CancelBookingCommand(ValidBookingId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithAlreadyCancelledBooking_ReturnsFailure()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();
        var clientMock = new Mock<IRoomClient>();

        var booking = CreateBooking(status: BookingStatus.Cancelled);

        repositoryMock
            .Setup(x => x.GetAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var handler = new CancelBookingCommandHandler(repositoryMock.Object, clientMock.Object);
        var command = new CancelBookingCommand(booking.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("already cancelled");
    }

    [Fact]
    public async Task Handle_WithCompletedBooking_ReturnsFailure()
    {
        // Arrange
        var repositoryMock = new Mock<IBookingRepository>();
        var clientMock = new Mock<IRoomClient>();

        var booking = CreateBooking(status: BookingStatus.Completed);

        repositoryMock
            .Setup(x => x.GetAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var handler = new CancelBookingCommandHandler(repositoryMock.Object, clientMock.Object);
        var command = new CancelBookingCommand(booking.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("completed");
    }
}