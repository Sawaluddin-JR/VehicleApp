using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleApp.Data;
using VehicleApp.Models;

namespace VehicleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleBrandController : ControllerBase
    {
        private readonly VehicleContext _context;

        public VehicleBrandController(VehicleContext context)
        {
            _context = context;
        }

        // GET: api/VehicleBrand
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VehicleBrand>>> GetVehicleBrands(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? filter = "")
        {
            IQueryable<VehicleBrand> query = _context.VehicleBrands;

            // Filtering by name if provided
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(b => b.Name.Contains(filter));
            }

            // Calculate total number of items
            var total = await query.CountAsync();

            // Apply pagination
            query = query.Skip((page - 1) * limit).Take(limit);

            var brands = await query.ToListAsync();

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
            return Ok(new { Metadata = metadata, Brands = brands });
        }

        // GET: api/VehicleBrand/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<VehicleBrand>> GetVehicleBrand(int id)
        {
            var vehicleBrand = await _context.VehicleBrands.FindAsync(id);

            if (vehicleBrand == null)
            {
                return NotFound();
            }

            return vehicleBrand;
        }

        // POST: api/VehicleBrand
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleBrand>> PostVehicleBrand(VehicleBrand vehicleBrand)
        {
            _context.VehicleBrands.Add(vehicleBrand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicleBrand), new { id = vehicleBrand.Id }, vehicleBrand);
        }

        // PATCH: api/VehicleBrand/5
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchVehicleBrand(int id, VehicleBrand vehicleBrand)
        {
            if (id != vehicleBrand.Id)
            {
                return BadRequest();
            }

            var existingBrand = await _context.VehicleBrands.FindAsync(id);
            if (existingBrand == null)
            {
                return NotFound();
            }

            existingBrand.Name = vehicleBrand.Name;
            existingBrand.UpdatedAt = DateTime.Now;

            _context.Entry(existingBrand).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/VehicleBrand/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicleBrand(int id)
        {
            var vehicleBrand = await _context.VehicleBrands.FindAsync(id);
            if (vehicleBrand == null)
            {
                return NotFound();
            }

            _context.VehicleBrands.Remove(vehicleBrand);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
