using Newtonsoft.Json;

namespace HttpWebshopCookie.Services;

public class PostalCodeService
{
    private readonly string jsonFilePath = Path.Combine("Data", "MockData", "postnummerfil.json");
    private readonly Dictionary<string, string> _postalCodeToCity;

    public PostalCodeService()
    {
        string jsonData = File.ReadAllText(jsonFilePath);
        var postalCodes = JsonConvert.DeserializeObject<List<PostalCodeEntry>>(jsonData) ?? [];

        _postalCodeToCity = [];
        foreach (var entry in postalCodes)
        {
            if (entry.PostalCode != null && !_postalCodeToCity.ContainsKey(entry.PostalCode))
            {
                _postalCodeToCity.Add(entry.PostalCode, entry.CityName ?? string.Empty);
            }
        }
    }

    public string GetCityByPostalCode(string postalCode)
    {
        if (_postalCodeToCity.TryGetValue(postalCode, out var cityName))
        {
            return cityName;
        }
        return string.Empty;
    }
}

public class PostalCodeEntry
{
    public string? PostalCode { get; set; }
    public string? CityName { get; set; }
}