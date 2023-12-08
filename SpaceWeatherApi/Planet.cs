using SpaceWeatherApi.Enum;

namespace SpaceWeatherApi;

public class Planet
{
    public string Name { get; set; }
    public WeatherForecast WeatherForecast { get; set; }
    public List<Satellite>? Satellites { get; set; }

    public Planet(string name, WeatherForecast weatherForecast, List<Satellite>? satellites)
    {
        Name = name;
        WeatherForecast = weatherForecast;
        Satellites = satellites;
    }
}
