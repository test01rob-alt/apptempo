using Newtonsoft.Json;

namespace apptempo.Models
{
    public class GeoResult
    {
        [JsonProperty("results")]
        public List<GeoLocation>? Results { get; set; }
    }

    public class GeoLocation
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }

    public class WeatherData
    {
        public string CityName { get; set; } = string.Empty;

        [JsonProperty("current_weather")]
        public CurrentWeather? CurrentWeather { get; set; }
    }

    public class CurrentWeather
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("windspeed")]
        public double WindSpeed { get; set; }

        [JsonProperty("weathercode")]
        public int WeatherCode { get; set; }

        public string Description => WeatherCode switch
        {
            0 => "Céu Limpo",
            1 or 2 or 3 => "Parcialmente Nublado",
            45 or 48 => "Nevoeiro",
            51 or 53 or 55 => "Garoa / Chuva Leve",
            61 or 63 or 65 => "Chuva Moderada",
            80 or 81 or 82 => "Pancadas de Chuva",
            95 or 96 or 99 => "Tempestade / Trovoada",
            _ => "Tempo Instável"
        };
    }
}