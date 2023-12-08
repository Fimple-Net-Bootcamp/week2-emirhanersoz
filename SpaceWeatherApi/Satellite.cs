namespace SpaceWeatherApi;

public class Satellite
{
    public string Name { get; set; }
    public WeatherForecast WeatherForecast { get; set; }

    public Satellite(string name, WeatherForecast weatherForecast)
    {
        Name = name;
        WeatherForecast = weatherForecast;
    }
}
