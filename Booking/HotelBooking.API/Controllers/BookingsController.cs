namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Tags("Bookings")]
public sealed class BookingsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Booking), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateBookingCommand(request.HotelId, request.UserId, 
                request.CheckIn, request.CheckOut, request.Guests, request.RoomIds);

            var response = await sender.Send(command, cancellationToken);
            return response.IsSuccessful
                ? Created("api/bookings", response.Data)
                : BadRequest(response.Message);
        }
        catch (HttpRequestException exception)
        {
            return Problem(
                title: "Hotel or Room service request failed.", 
                detail: exception.Message, 
                statusCode: StatusCodes.Status502BadGateway);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Booking>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetBookingsQuery();
        var response = await sender.Send(query, cancellationToken);
        return response.IsSuccessful
            ? Ok(response.Data)
            : NotFound(response.Message);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBookingDetailsQuery(id);
        var response = await sender.Send(query, cancellationToken);
        return response.IsSuccessful
            ? Ok(response.Data)
            : NotFound(response.Message);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelBookingCommand(id);
            var response = await sender.Send(command, cancellationToken);
            return response.IsSuccessful
                ? Ok(response.Data)
                : NotFound(response.Message);
        }
        catch (HttpRequestException exception)
        {
            return Problem(
                title: "Hotel or Room service request failed.",
                detail: exception.Message,
                statusCode: StatusCodes.Status502BadGateway);
        }
    }
}