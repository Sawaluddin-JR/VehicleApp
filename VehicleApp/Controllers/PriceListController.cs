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
    public class PriceListController : ControllerBase
    {
        private readonly VehicleContext _context;

        public PriceListController(VehicleContext context)
        {
            _context = context;
        }

        // GET: api/PriceList
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PriceList>>> GetPriceLists(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] int yearId = 0,
            [FromQuery] int modelId = 0)
        {
            IQueryable<PriceList> query = _context.PriceLists
                .Include(p => p.Year)
                .Include(p => p.Model);

            // Filtering by yearId if provided
            if (yearId != 0)
            {
                query = query.Where(p => p.YearId == yearId);
            }

            // Filtering by modelId if provided
            if (modelId != 0)
            {
                query = query.Where(p => p.ModelId == modelId);
            }

            // Calculate total number of items
            var total = await query.CountAsync();

            // Apply pagination
            query = query.Skip((page - 1) * limit).Take(limit);

            var priceLists = await query.ToListAsync();

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
            return Ok(new { Metadata = metadata, PriceLists = priceLists });
        }

        // GET: api/PriceList/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PriceList>> GetPriceList(int id)
        {
            var priceList = await _context.PriceLists
                .Include(p => p.Year)
                .Include(p => p.Model)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (priceList == null)
            {
                return NotFound();
            }

            return priceList;
        }

        // POST: api/PriceList
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PriceList>> PostPriceList(PriceList priceList)
        {
            _context.PriceLists.Add(priceList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPriceList), new { id = priceList.Id }, priceList);
        }

        // PATCH: api/PriceList/5
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchPriceList(int id, PriceList priceList)
        {
            if (id != priceList.Id)
            {
                return BadRequest();
            }

            var existingPriceList = await _context.PriceLists.FindAsync(id);
            if (existingPriceList == null)
            {
                return NotFound();
            }

            existingPriceList.Code = priceList.Code;
            existingPriceList.Price = priceList.Price;
            existingPriceList.YearId = priceList.YearId;
            existingPriceList.ModelId = priceList.ModelId;
            existingPriceList.UpdatedAt = DateTime.Now;

            _context.Entry(existingPriceList).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PriceList/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePriceList(int id)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null)
            {
                return NotFound();
            }

            _context.PriceLists.Remove(priceList);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
