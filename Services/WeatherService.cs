using System.Net;
using Newtonsoft.Json;
using apptempo.Models;

namespace apptempo.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
            // Adicionar User-Agent é fundamental para evitar bloqueios de segurança (HTTP 403) em APIs públicas
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AppTempoMaui/1.0 (MobileApp)");
        }

        public async Task<WeatherData> ObterClimaAsync(string cidade)
        {
            // 1. Verificação de Conexão com a Internet (Atividade 2)
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new Exception("SEM_CONEXAO");
            }

            try
            {
                // 2. Busca da Latitude e Longitude da cidade via Geocoding
                string geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(cidade)}&count=1&language=pt";

                HttpResponseMessage geoResponse = await _httpClient.GetAsync(geoUrl);

                if (!geoResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro na API de busca ({geoResponse.StatusCode})");
                }

                string geoJson = await geoResponse.Content.ReadAsStringAsync();
                var geoData = JsonConvert.DeserializeObject<GeoResult>(geoJson);

                // Trata cidade não encontrada (Atividade 2)
                if (geoData?.Results == null || geoData.Results.Count == 0)
                {
                    throw new Exception("CIDADE_NAO_ENCONTRADA");
                }

                var local = geoData.Results[0];

                // 3. Busca dos dados meteorológicos com a coordenada obtida
                string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={local.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={local.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&current_weather=true";

                HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherUrl);

                if (weatherResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("CIDADE_NAO_ENCONTRADA");
                }

                if (weatherResponse.IsSuccessStatusCode)
                {
                    string weatherJson = await weatherResponse.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<WeatherData>(weatherJson);

                    if (weatherData != null)
                    {
                        weatherData.CityName = local.Name ?? cidade;
                        return weatherData;
                    }
                }

                throw new Exception("Não foi possível interpretar a resposta meteorológica.");
            }
            catch (Exception ex) when (ex.Message != "SEM_CONEXAO" && ex.Message != "CIDADE_NAO_ENCONTRADA")
            {
                throw new Exception($"Detalhe da Falha: {ex.Message}");
            }
        }
    }
}