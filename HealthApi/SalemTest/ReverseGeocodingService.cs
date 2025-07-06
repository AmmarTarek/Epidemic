using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

public class ReverseGeocodingService
{
    private readonly HttpClient _httpClient;

    public ReverseGeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetOSMAreaName(double lat, double lon)
    {
        var url = $"https://nominatim.openstreetmap.org/reverse?lat={lat}&lon={lon}&format=json";

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName"); // Required by OSM

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        var root = doc.RootElement;
        var displayName = root.GetProperty("display_name").GetString();

        return displayName ?? "Unknown Location";
    }
}
