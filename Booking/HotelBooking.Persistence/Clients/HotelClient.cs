namespace HotelBooking.Persistence.Clients;

public sealed class HotelClient(HttpClient httpClient) : IHotelClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    
    public async Task<HotelSnapshot?> GetHotel(Guid hotelId, CancellationToken cancellationToken)
    {
        using var responseMessage = await httpClient.GetAsync($"api/hotels/{hotelId}", cancellationToken);
        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            return null;

        responseMessage.EnsureSuccessStatusCode();

        var response = await responseMessage.Content
            .ReadFromJsonAsync<Response<HotelSnapshot>>(JsonOptions, cancellationToken);

        return response?.Data is null
            ? null
            : new HotelSnapshot(response.Data.Id, response.Data.Name, response.Data.IsExcluded);
    }
}