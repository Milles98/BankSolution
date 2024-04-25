using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BankWeb.ViewComponents
{
    public class WeatherViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _cache;

        public WeatherViewComponent(IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            _clientFactory = clientFactory;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_cache.TryGetValue("Weather", out WeatherModel weatherData))
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync("http://api.weatherapi.com/v1/current.json?key=abe8320defb74b72adf111532242504&q=Sweden");

                weatherData = new WeatherModel();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsAsync<ApiResponse>();
                    weatherData.Location = apiResponse.location.name;
                    weatherData.Temperature = apiResponse.current.temp_c.ToString();
                    _cache.Set("Weather", weatherData, TimeSpan.FromMinutes(15));
                }
                else
                {
                    weatherData.Location = "Weather";
                    weatherData.Temperature = "Unavailable";
                }
            }

            return View(weatherData);
        }
    }

}
