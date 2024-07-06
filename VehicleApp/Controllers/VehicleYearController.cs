using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehicleApp.Data;
using VehicleApp.Models;

namespace VehicleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class VehicleYearController : ControllerBase
    {
        private readonly VehicleContext _context;
        private readonly ILogger<VehicleYearController> _logger;

        public VehicleYearController(VehicleContext context, ILogger<VehicleYearController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/VehicleYear
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleYear>>> GetVehicleYears(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? filter = null)
        {
            IQueryable<VehicleYear> query = _context.VehicleYears;

            // Apply filtering
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(vy => vy.Year.ToString().Contains(filter));
            }

            // Calculate total number of items
            var total = await query.CountAsync();

            // Apply pagination
            query = query.Skip((page - 1) * limit).Take(limit);

            var years = await query.ToListAsync();

            // Metadata for pagination
            var metadata = new
            {
                Total = total,
                Limit = limit,
                Page = page,
                NextPage = page * limit < total ? (int?)page + 1 : null,
                PrevPage = page > 1 ? (int?)page - 1 : null
            };

            // Return response with pagination metadata
            return Ok(new { Metadata = metadata, Years = years });
        }

        // GET: api/VehicleYear/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleYear>> GetVehicleYear(int id)
        {
            var vehicleYear = await _context.VehicleYears.FindAsync(id);

            if (vehicleYear == null)
            {
                return NotFound();
            }

            return vehicleYear;
        }

        // POST: api/VehicleYear
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleYear>> PostVehicleYear(VehicleYear vehicleYear)
        {
            try
            {
                vehicleYear.CreatedAt = DateTime.Now;
                vehicleYear.UpdatedAt = DateTime.Now;
                _context.VehicleYears.Add(vehicleYear);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetVehicleYear), new { id = vehicleYear.Id }, vehicleYear);
            }
            catch (Exception ex)
            {
                // Log the error details
                _logger.LogError(ex, "An error occurred while creating a vehicle year.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Failed to create vehicle year: {ex.Message}" });
            }
        }

        // PATCH: api/VehicleYear/5
        [HttpPatch("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchVehicleYear(int id, VehicleYear vehicleYear)
        {
            if (id != vehicleYear.Id)
            {
                return BadRequest();
            }

            var existingYear = await _context.VehicleYears.FindAsync(id);
            if (existingYear == null)
            {
                return NotFound();
            }

            existingYear.Year = vehicleYear.Year;
            existingYear.UpdatedAt = DateTime.Now;

            _context.Entry(existingYear).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/VehicleYear/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicleYear(int id)
        {
            var vehicleYear = await _context.VehicleYears.FindAsync(id);
            if (vehicleYear == null)
            {
                return NotFound();
            }

            _context.VehicleYears.Remove(vehicleYear);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
