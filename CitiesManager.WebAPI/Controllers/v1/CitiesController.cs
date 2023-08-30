using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.WebAPI.DatabaseContext;
using CitiesManager.WebAPI.Models;
using Microsoft.AspNetCore.Cors;

namespace CitiesManager.WebAPI.Controllers.v1;

////[ApiController] //ako ne stavimo [ApiController] onda moramo u sve PUT i POST staviti [FromBody]
[ApiVersion("1.0")]
//[EnableCors("4100Client")] //da bi omogucili custom cors policy za odredjeni controller
public class CitiesController : CustomControllerBase
{
    private readonly ApplicationDbContext _context;

    public CitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Cities
    /// <summary>
    /// To get list of cities (including city ID and city name) from 'cities' table
    /// </summary>
    /// <returns>Return city object</returns>
    [HttpGet] //ne treba ako action method pocinje sa Get (GetCities())
    //[Produces("application/xml")] // da Content-type bude:application/xml
    public async Task<ActionResult<IEnumerable<City>>> GetCities()
    {
        if (_context.Cities == null)
        {
            return NotFound();
        }
        var cities = await _context.Cities.OrderBy(tepm => tepm.CityName).ToListAsync();
        return cities;
    }

    /// <summary>
    /// To get individual object of cities list (including city ID and city name) from 'cities' table
    /// </summary>
    /// <param name="cityId"></param>
    /// <returns></returns>
    // GET: api/Cities/5
    [HttpGet("{cityId}")] // kombinovano donja dva
    //[HttpGet]
    //[Route("{cityId}")]
    //public async Task<IActionResult> GetCity(Guid cityId)
    public async Task<ActionResult<City>> GetCity(Guid cityId)
    {
        //if (_context.Cities == null)
        //{
        //    return NotFound();
        //} nece nikad biti null posto potice od ef

        //var city = await _context.Cities.FindAsync(cityId);
        var city = await _context.Cities.FirstOrDefaultAsync(temp => temp.CityId == cityId);

        if (city == null)
        {
            //Response.StatusCode = 404;
            //return NotFound();
            //return BadRequest();
            return Problem("Invalid CityId", statusCode: 400, title: "City Search");
        }

        //return Ok(city); - ako koristimo IActionResult
        return city;
    }

    /// <summary>
    /// To edit individual object of cities list  from 'cities' table
    /// </summary>
    /// <param name="cityId"></param>
    /// <param name="city"></param>
    /// <returns></returns>
    // PUT: api/Cities/5
    [HttpPut("{cityId}")]
    public async Task<IActionResult> PutCity(Guid cityId, [Bind(nameof(City.CityId), nameof(City.CityName))] City city)
    {
        if (cityId != city.CityId)
        {
            return BadRequest();
        }

        //_context.Entry(city).State = EntityState.Modified; - na ovaj nacin updetujemo sve elemente

        var existingCity = await _context.Cities.FindAsync(cityId);
        if (existingCity == null)
        {
            return NotFound(); //HTTP 404
        }
        existingCity.CityName = city.CityName;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) // ako je paraleno (u isto vrijeme) updetovan objekat od drugog usera
        {
            if (!CityExists(cityId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    ///  To create object of cities in 'cities' table
    /// </summary>
    /// <param name="city"></param>
    /// <returns></returns>
    // POST: api/Cities
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<City>> PostCity([Bind(nameof(City.CityId), nameof(City.CityName))] City city)
    {
        //if (ModelState.IsValid == false)
        //{
        //    return ValidationProblem(ModelState);
        //} -> automatski se radi validacija preko controllera
        if (_context.Cities == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Cities'  is null.");
        }
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCity", new { cityId = city.CityId }, city); //api/cities/{cityId}
    }

    /// <summary>
    /// To delete individual object of cities in 'cities' table
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // DELETE: api/Cities/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        //if (_context.Cities == null)
        //{
        //    return NotFound(); 
        //}
        var city = await _context.Cities.FindAsync(id);
        if (city == null)
        {
            return NotFound();//HTTP 404
        }

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync();

        return NoContent(); //HTTP 200
    }

    private bool CityExists(Guid id)
    {
        return (_context.Cities?.Any(e => e.CityId == id)).GetValueOrDefault();
    }
}
