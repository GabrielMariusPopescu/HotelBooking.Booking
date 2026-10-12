namespace HotelBooking.Booking.API.Tests.Commands;

[ExcludeFromCodeCoverage]
public class CreateBookingCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_WithValidRequest_ReturnsSuccessfulResponse()
    {
        // Arrange
        var hotelClientMock = new Mock<IHotelClient>();
        var roomClientMock = new Mock<IRoomClient>();
        var repositoryMock = new Mock<IBookingRepository>();

        var hotelSnapshot = CreateHotelSnapshot();
        var roomSnapshot = CreateRoomSnapshot();
        var reservationSnapshot = CreateRoomReservationSnapshot();
        var booking = CreateBooking();

        hotelClientMock
            .Setup(x => x.GetHotel(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelSnapshot);

        roomClientMock
            .Setup(x => x.GetRooms(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([roomSnapshot]);

        roomClientMock
            .Setup(x => x.Reserve(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservationSnapshot);

        repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Models.Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var handler = new CreateBookingCommandHandler(hotelClientMock.Object, roomClientMock.Object, repositoryMock.Object);

        var command = new CreateBookingCommand(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.HotelId.Should().Be(ValidHotelId);
        result.Data.UserId.Should().Be(ValidUserId);
        result.Data.Status.Should().Be(BookingStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_WithInvalidHotelId_ReturnsFailureResponse()
    {
        // Arrange
        var hotelClientMock = new Mock<IHotelClient>();
        var roomClientMock = new Mock<IRoomClient>();
        var repositoryMock = new Mock<IBookingRepository>();

        hotelClientMock
            .Setup(x => x.GetHotel(Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HotelSnapshot?)null);

        var handler = new CreateBookingCommandHandler(hotelClientMock.Object, roomClientMock.Object, repositoryMock.Object);

        var command = new CreateBookingCommand(
            Guid.Empty,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("required");
    }

    [Fact]
    public async Task Handle_WithExcludedHotel_ReturnsFailureResponse()
    {
        // Arrange
        var hotelClientMock = new Mock<IHotelClient>();
        var roomClientMock = new Mock<IRoomClient>();
        var repositoryMock = new Mock<IBookingRepository>();

        var excludedHotel = CreateHotelSnapshot(isExcluded: true);

        hotelClientMock
            .Setup(x => x.GetHotel(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(excludedHotel);

        var handler = new CreateBookingCommandHandler(hotelClientMock.Object, roomClientMock.Object, repositoryMock.Object);

        var command = new CreateBookingCommand(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("not found or is excluded");
    }

    [Fact]
    public async Task Handle_WithPastCheckInDate_ReturnsFailureResponse()
    {
        // Arrange
        var hotelClientMock = new Mock<IHotelClient>();
        var roomClientMock = new Mock<IRoomClient>();
        var repositoryMock = new Mock<IBookingRepository>();

        var handler = new CreateBookingCommandHandler(hotelClientMock.Object, roomClientMock.Object, repositoryMock.Object);

        var pastCheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var futureCheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var command = new CreateBookingCommand(
            ValidHotelId,
            ValidUserId,
            pastCheckIn,
            futureCheckOut,
            GuestCount,
            [ValidRoomId]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("today or later");
    }

    [Fact]
    public async Task Handle_WithNoRooms_ReturnsFailureResponse()
    {
        // Arrange
        var hotelClientMock = new Mock<IHotelClient>();
        var roomClientMock = new Mock<IRoomClient>();
        var repositoryMock = new Mock<IBookingRepository>();

        var hotelSnapshot = CreateHotelSnapshot();

        hotelClientMock
            .Setup(x => x.GetHotel(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelSnapshot);

        roomClientMock
            .Setup(x => x.GetRooms(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RoomSnapshot>());

        var handler = new CreateBookingCommandHandler(hotelClientMock.Object, roomClientMock.Object, repositoryMock.Object);

        var command = new CreateBookingCommand(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("No rooms found");
    }

    [Fact]
    public async Task Handle_WithUnavailableRoom_ReturnsFailureResponse()
    {
        // Arrange
        var hotelClientMock = new Mock<IHotelClient>();
        var roomClientMock = new Mock<IRoomClient>();
        var repositoryMock = new Mock<IBookingRepository>();

        var hotelSnapshot = CreateHotelSnapshot();
        var unavailableRoom = CreateRoomSnapshot(roomStatus: "Booked");

        hotelClientMock
            .Setup(x => x.GetHotel(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelSnapshot);

        roomClientMock
            .Setup(x => x.GetRooms(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([unavailableRoom]);

        var handler = new CreateBookingCommandHandler(hotelClientMock.Object, roomClientMock.Object, repositoryMock.Object);

        var command = new CreateBookingCommand(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("not available");
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_CancelsReservation()
    {
        // Arrange
        var hotelClientMock = new Mock<IHotelClient>();
        var roomClientMock = new Mock<IRoomClient>();
        var repositoryMock = new Mock<IBookingRepository>();

        var hotelSnapshot = CreateHotelSnapshot();
        var roomSnapshot = CreateRoomSnapshot();
        var reservationSnapshot = CreateRoomReservationSnapshot();

        hotelClientMock
            .Setup(x => x.GetHotel(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelSnapshot);

        roomClientMock
            .Setup(x => x.GetRooms(ValidHotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([roomSnapshot]);

        roomClientMock
            .Setup(x => x.Reserve(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservationSnapshot);

        repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Models.Booking>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB Error"));

        var handler = new CreateBookingCommandHandler(hotelClientMock.Object, roomClientMock.Object, repositoryMock.Object);

        var command = new CreateBookingCommand(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));

        roomClientMock.Verify(
            x => x.CancelReservation(ValidReservationId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}