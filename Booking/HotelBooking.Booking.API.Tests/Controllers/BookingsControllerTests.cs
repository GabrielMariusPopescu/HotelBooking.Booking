namespace HotelBooking.Booking.API.Tests.Controllers;

[ExcludeFromCodeCoverage]
public class BookingsControllerTests : TestBase
{
    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var booking = CreateBooking();
        var response = Response<Domain.Models.Booking>.Success(booking);

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        var request = new CreateBookingRequest(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);


        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.Location.Should().Contain("api/bookings");
    }

    [Fact]
    public async Task Create_WithFailedResponse_ReturnsBadRequest()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var response = Response<Domain.Models.Booking>.Failure("Hotel not found");

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        var request = new CreateBookingRequest(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);


        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Create_WhenHttpRequestExceptionThrown_ReturnsBadGateway()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        var controller = new BookingsController(mediatorMock.Object);

        var request = new CreateBookingRequest(
            ValidHotelId,
            ValidUserId,
            CheckInDate,
            CheckOutDate,
            GuestCount,
            [ValidRoomId]);


        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(502);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithBookings()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var bookings = new List<Domain.Models.Booking> { CreateBooking(), CreateBooking() };
        var response = Response<IReadOnlyCollection<Domain.Models.Booking>>.Success(bookings);

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetBookingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetAll_WhenNoBookingsFound_ReturnsNotFound()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var response = Response<IReadOnlyCollection<Domain.Models.Booking>>.Failure("No bookings found");

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetBookingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Get_WithValidId_ReturnsOkWithBooking()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var booking = CreateBooking();
        var response = Response<Domain.Models.Booking>.Success(booking);

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetBookingDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        // Act
        var result = await controller.Get(booking.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var response = Response<Domain.Models.Booking>.Failure("Booking not found");

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetBookingDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        // Act
        var result = await controller.Get(ValidBookingId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Cancel_WithValidId_ReturnsOkWithCancelledBooking()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var booking = CreateBooking(status: BookingStatus.Cancelled);
        var response = Response<Domain.Models.Booking>.Success(booking);

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CancelBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        // Act
        var result = await controller.Cancel(booking.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Cancel_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var response = Response<Domain.Models.Booking>.Failure("Booking not found");

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CancelBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new BookingsController(mediatorMock.Object);

        // Act
        var result = await controller.Cancel(ValidBookingId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Cancel_WhenHttpRequestExceptionThrown_ReturnsBadGateway()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CancelBookingCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        var controller = new BookingsController(mediatorMock.Object);

        // Act
        var result = await controller.Cancel(ValidBookingId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(502);
    }
}