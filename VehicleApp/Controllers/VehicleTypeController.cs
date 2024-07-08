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
    public class VehicleTypeController : ControllerBase
    {
        private readonly VehicleContext _context;

        public VehicleTypeController(VehicleContext context)
        {
            _context = context;
        }

        // GET: api/VehicleType
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VehicleType>>> GetVehicleTypes(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] int brandId = 0)
        {
            IQueryable<VehicleType> query = _context.VehicleTypes.Include(vt => vt.Brand);

            // Filtering by brandId if provided
            if (brandId != 0)
            {
                query = query.Where(vt => vt.BrandId == brandId);
            }

            // Calculate total number of items
            var total = await query.CountAsync();

            // Apply pagination
            query = query.Skip((page - 1) * limit).Take(limit);

            var types = await query.ToListAsync();

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
            return Ok(new { Metadata = metadata, Types = types });
        }

        // GET: api/VehicleType/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<VehicleType>> GetVehicleType(int id)
        {
            var vehicleType = await _context.VehicleTypes.FindAsync(id);

            if (vehicleType == null)
            {
                return NotFound();
            }

            return vehicleType;
        }

        // POST: api/VehicleType
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleType>> PostVehicleType(VehicleType vehicleType)
        {
            vehicleType.CreatedAt = DateTime.UtcNow;
            vehicleType.UpdatedAt = DateTime.UtcNow;

            if (vehicleType.BrandId != 0)
            {
                vehicleType.Brand = await _context.VehicleBrands.FindAsync(vehicleType.BrandId);
                if (vehicleType.Brand == null)
                {
                    return BadRequest(new { message = "Invalid BrandId provided." });
                }
            }

            _context.VehicleTypes.Add(vehicleType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicleType), new { id = vehicleType.Id }, vehicleType);
        }

        // PATCH: api/VehicleType/5
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchVehicleType(int id, VehicleType vehicleType)
        {
            if (id != vehicleType.Id)
            {
                return BadRequest();
            }

            var existingType = await _context.VehicleTypes.FindAsync(id);
            if (existingType == null)
            {
                return NotFound();
            }

            existingType.Name = vehicleType.Name;
            existingType.BrandId = vehicleType.BrandId;
            existingType.UpdatedAt = DateTime.Now;

            _context.Entry(existingType).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/VehicleType/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicleType(int id)
        {
            var vehicleType = await _context.VehicleTypes.FindAsync(id);
            if (vehicleType == null)
            {
                return NotFound();
            }

            _context.VehicleTypes.Remove(vehicleType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
