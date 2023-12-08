using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpaceWeatherApi.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherController : ControllerBase
{
    private readonly List<Planet> _planets;

    public WeatherController()
    {
        _planets = new List<Planet>
        {
                // Uydusu bulunmayan gezegenler
                new Planet("Venus", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = 100, Condition = Enum.WeatherCondition.Cloudy }, null),
                new Planet("Mercury", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = 430, Condition = Enum.WeatherCondition.ExtremeHeat, Summary = "No one can live in this temperature" }, null),

                //Uydusu bulunan gezegenler
                new Planet("Earth", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = 15, Condition = Enum.WeatherCondition.Mild }, new List<Satellite>
                {
                    new Satellite("Moon", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = 100, Condition = Enum.WeatherCondition.Stormy }),
                }),

                new Planet("Mars", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -30, Condition = Enum.WeatherCondition.Mild, Summary = "No viewing angle" }, new List<Satellite>
                {
                    new Satellite("Phobos", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -130, Condition = Enum.WeatherCondition.Scorching, Summary = "It's freezing cold" }),
                    new Satellite("Deimos", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -25, Condition = Enum.WeatherCondition.Windy})
                }),

                new Planet("Jupiter", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -160, Condition = Enum.WeatherCondition.Stormy, Summary = "The wind is destructive" }, new List<Satellite>
                {
                    new Satellite("Io", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -80, Condition = Enum.WeatherCondition.Stormy}),
                    new Satellite("Europa", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -170, Condition = Enum.WeatherCondition.Ringy, Summary = "Hail is falling from the clouds" })
                }),

                new Planet("Saturn", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -185, Condition = Enum.WeatherCondition.Windy}, new List<Satellite>
                {
                    new Satellite("Titan", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -185, Condition = Enum.WeatherCondition.Scorching}),
                    new Satellite("Enceladus", new WeatherForecast { Date = new DateOnly(2023, 12, 8), TemperatureC = -175, Condition = Enum.WeatherCondition.ExtremeCold, Summary = "Rains can kill living things" })
                })

        };
    }

    //Gezegenlerin verisini okuma
    [HttpGet("{planetName}")]
    [ProducesResponseType(typeof(Planet), 200)]
    public IActionResult GetPlanetWeather(string planetName)
    {
        var planet = _planets.FirstOrDefault(g => g.Name.ToLower() == planetName.ToLower());

        if (planet == null)
        {
            return NotFound(); // Belirtilen gezegen bulunamadı.
        }

        return Ok(new { planet.WeatherForecast, planet.Satellites });
    }

    //Uyduların verisini okuma
    [HttpGet("{planetName}/{satelliteName}")]
    public IActionResult GetSatelliteWeather(string planetName, string satelliteName)
    {
        var planet = _planets.FirstOrDefault(g => g.Name.ToLower() == planetName.ToLower());

        if (planet == null)
        {
            return NotFound(); // Belirtilen gezegen bulunamadı.
        }

        var satellite = planet.Satellites.FirstOrDefault(u => u.Name.ToLower() == satelliteName.ToLower());

        if (satellite == null)
        {
            return NotFound(); // Belirtilen uydu bulunamadı.
        }

        return Ok(new {Planet = planet.WeatherForecast, Satellite = satellite.WeatherForecast });
    }

    //Yeni gezegen ekleme
    [HttpPost]
    [ProducesResponseType(typeof(Planet), 201)]
    public IActionResult AddPlanet([FromBody] Planet planet)
    {
        _planets.Add(planet);
        return CreatedAtAction(nameof(GetPlanetWeather), new { planetName = planet.Name }, planet);
    }

    //Gezegenlerin hava durumunu güncelleme
    [HttpPut("{planetName}")]
    public IActionResult UpdatePlanetWeather(string planetName, [FromBody] WeatherForecast newWeatherForecast)
    {
        var planet = _planets.FirstOrDefault(g => g.Name.ToLower() == planetName.ToLower());

        if (planet == null)
        {
            return NotFound(); // Belirtilen gezegen bulunamadı.
        }

        planet.WeatherForecast = newWeatherForecast;

        return Ok(new { planet.WeatherForecast, planet.Satellites });
    }

    //Gezegen hava durumu bilgilerini kısmnen güncelleme
    [HttpPatch("{planetName}")]
    public IActionResult PartialUpdatePlanetWeather(string planetName, [FromBody] JsonPatchDocument<WeatherForecast> patch)
    {
        var planet = _planets.FirstOrDefault(g => g.Name.ToLower() == planetName.ToLower());

        if (planet == null)
        {
            return NotFound(); // Belirtilen gezegen bulunamadı.
        }

        patch.ApplyTo(planet.WeatherForecast, ModelState);

        return Ok(new { planet.WeatherForecast, planet.Satellites });
    }

    //Gezegen silme
    [HttpDelete("{planetName}")]
    [ProducesResponseType(204)]
    public IActionResult DeletePlanet(string planetName)
    {
        var planet = _planets.FirstOrDefault(g => g.Name.ToLower() == planetName.ToLower());

        if (planet == null)
        {
            return NotFound(); // Belirtilen gezegen bulunamadı.
        }

        _planets.Remove(planet);

        return NoContent();
    }

    // Gezegenleri sayfalama, filtreleme, sıralama
    [HttpGet]
    public IActionResult GetPlanets([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string sort = null, [FromQuery] string nameFilter = null)
    {
        // Sayfalama
        var paginatedPlanets = _planets.Skip((page - 1) * size).Take(size);

        // Filtreleme (örnek: isme göre filtreleme)
        if (!string.IsNullOrEmpty(nameFilter))
        {
            paginatedPlanets = paginatedPlanets.Where(p => p.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
        }

        // Sıralama
        if (!string.IsNullOrEmpty(sort))
        {
            var sortParams = sort.Split(',');
            if (sortParams.Length == 2)
            {
                var sortField = sortParams[0];
                var isAscending = sortParams[1].ToLower() == "asc";

                paginatedPlanets = SortPlanets(paginatedPlanets, sortField, isAscending);
            }
        }

        return Ok(paginatedPlanets);
    }

    private IEnumerable<Planet> SortPlanets(IEnumerable<Planet> planets, string field, bool isAscending)
    {
        switch (field.ToLower())
        {
            case "name":
                return isAscending ? planets.OrderBy(p => p.Name) : planets.OrderByDescending(p => p.Name);
            default:
                return planets;
        }
    }
}
