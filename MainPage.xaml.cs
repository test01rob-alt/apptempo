using apptempo.Services;
using apptempo.Models;

namespace apptempo
{
    public partial class MainPage : ContentPage
    {
        private readonly WeatherService _weatherService;

        public MainPage()
        {
            InitializeComponent();
            _weatherService = new WeatherService();
        }

        private async void OnBuscarClimaClicked(object? sender, EventArgs e)
        {
            string cidade = txtCidade.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(cidade))
            {
                await DisplayAlertAsync("Atenção", "Por favor, digite o nome de uma cidade.", "OK");
                return;
            }

            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            borderResultado.IsVisible = false;

            try
            {
                WeatherData dados = await _weatherService.ObterClimaAsync(cidade);

                lblCidade.Text = $"🌍 {dados.CityName}";

                string condicao = dados.CurrentWeather?.Description ?? "Sem informação";
                lblCondicao.Text = $"🌤️ {condicao.ToUpper()}";

                double temp = dados.CurrentWeather?.Temperature ?? 0;
                double vento = dados.CurrentWeather?.WindSpeed ?? 0;

                lblTemperatura.Text = $"🌡️ Temperatura: {temp:F1} °C";
                lblVento.Text = $"💨 Vento: {vento:F1} km/h";
                lblVisibilidade.Text = "👁️ Visibilidade: 10.0 km (Padrão)";

                borderResultado.IsVisible = true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "SEM_CONEXAO")
                {
                    await DisplayAlertAsync("Sem Conexão", "Você está sem acesso à internet. Verifique sua rede e tente novamente.", "OK");
                }
                else if (ex.Message == "CIDADE_NAO_ENCONTRADA")
                {
                    await DisplayAlertAsync("Não Encontrado", $"A cidade '{cidade}' não foi encontrada. Verifique a grafia.", "OK");
                }
                else
                {
                    await DisplayAlertAsync("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
                }
            }
            finally
            {
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            }
        }
    }
}