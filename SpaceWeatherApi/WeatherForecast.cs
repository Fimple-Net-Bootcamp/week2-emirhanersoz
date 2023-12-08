using SpaceWeatherApi.Enum;

namespace SpaceWeatherApi
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public WeatherCondition Condition { get; set; } = WeatherCondition.Unknown;

        public string? Summary { get; set; }
    }
}
