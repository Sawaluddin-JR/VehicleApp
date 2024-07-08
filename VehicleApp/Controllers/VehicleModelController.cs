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
    public class VehicleModelController : ControllerBase
    {
        private readonly VehicleContext _context;

        public VehicleModelController(VehicleContext context)
        {
            _context = context;
        }

        // GET: api/VehicleModel
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VehicleModel>>> GetVehicleModels(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? filter = null)
        {
            IQueryable<VehicleModel> query = _context.VehicleModels;

            // Apply filtering
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(vm => vm.Name.Contains(filter));
            }

            // Calculate total number of items
            var total = await query.CountAsync();

            // Apply pagination
            query = query.Skip((page - 1) * limit).Take(limit);

            var models = await query.ToListAsync();

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
            return Ok(new { Metadata = metadata, Models = models });
        }

        // GET: api/VehicleModel/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<VehicleModel>> GetVehicleModel(int id)
        {
            var vehicleModel = await _context.VehicleModels.FindAsync(id);

            if (vehicleModel == null)
            {
                return NotFound();
            }

            return vehicleModel;
        }

        // POST: api/VehicleModel
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleModel>> PostVehicleModel(VehicleModel vehicleModel)
        {
            vehicleModel.CreatedAt = DateTime.UtcNow;
            vehicleModel.UpdatedAt = DateTime.UtcNow;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicleType = await _context.VehicleTypes.FindAsync(vehicleModel.TypeId);
            if (vehicleType == null)
            {
                return BadRequest(new { message = "Invalid TypeId provided." });
            }

            _context.VehicleModels.Add(vehicleModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicleModel), new { id = vehicleModel.Id }, vehicleModel);
        }

        // PATCH: api/VehicleModel/5
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchVehicleModel(int id, VehicleModel vehicleModel)
        {
            if (id != vehicleModel.Id)
            {
                return BadRequest();
            }

            var existingModel = await _context.VehicleModels.FindAsync(id);
            if (existingModel == null)
            {
                return NotFound();
            }

            var vehicleType = await _context.VehicleTypes.FindAsync(vehicleModel.TypeId);
            if (vehicleType == null)
            {
                return BadRequest(new { message = "Invalid TypeId provided." });
            }
;
            existingModel.Name = vehicleModel.Name;
            existingModel.TypeId = vehicleModel.TypeId;
            existingModel.UpdatedAt = DateTime.Now;

            _context.Entry(existingModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/VehicleModel/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicleModel(int id)
        {
            var vehicleModel = await _context.VehicleModels.FindAsync(id);
            if (vehicleModel == null)
            {
                return NotFound();
            }

            _context.VehicleModels.Remove(vehicleModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}