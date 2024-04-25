using Microsoft.AspNetCore.Mvc;

namespace BankWeb.ViewComponents
{
    public class WeatherViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _clientFactory;

        public WeatherViewComponent(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("http://api.weatherapi.com/v1/current.json?key=abe8320defb74b72adf111532242504&q=Sweden");

            var weatherData = new WeatherModel();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsAsync<ApiResponse>();
                weatherData.Location = apiResponse.location.name;
                weatherData.Temperature = apiResponse.current.temp_c.ToString();
            }
            else
            {
                weatherData.Location = "Unknown";
                weatherData.Temperature = "Unknown";
            }

            return View(weatherData);
        }

    }
}
