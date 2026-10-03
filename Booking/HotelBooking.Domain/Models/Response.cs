namespace HotelBooking.Domain.Models;

public record Response<T>(bool IsSuccessful, string Message, T? Data)
{
    public static Response<T> Success(T data)
        => new(true, string.Empty, data);

    public static Response<T> Failure(string message)
        => new(false, message, default);
}