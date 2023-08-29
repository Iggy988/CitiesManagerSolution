using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.WebAPI.DatabaseContext;
using CitiesManager.WebAPI.Models;

namespace CitiesManager.WebAPI.Controllers.v2;

//[Route("api/[controller]")]
//[ApiController] //ako ne stavimo [ApiController] onda moramo u sve PUT i POST staviti [FromBody]
//gore ne treba, stavili smo u CustomControllerBase
[ApiVersion("2.0")]
public class CitiesController : CustomControllerBase
{
    private readonly ApplicationDbContext _context;

    public CitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Cities
    /// <summary>
    /// To get list of cities (only city name) from 'cities' table
    /// </summary>
    /// <returns>Return city object</returns>
    [HttpGet] //ne treba ako action method pocinje sa Get (GetCities())
    //[Produces("application/xml")] // da Content-type bude:application/xml
    public async Task<ActionResult<IEnumerable<string?>>> GetCities()
    {
        if (_context.Cities == null)
        {
            return NotFound();
        }
        var cities = await _context.Cities
            .OrderBy(tepm => tepm.CityName)
            .Select(temp => temp.CityName)
            .ToListAsync();
        return cities;
    }
}
