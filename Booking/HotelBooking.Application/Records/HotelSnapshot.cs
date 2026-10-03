namespace HotelBooking.Application.Records;

public sealed record HotelSnapshot(Guid Id, string Name, bool IsExcluded);