using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_order_documentor_netcore;
using online_order_documentor_netcore.Models;

namespace online_order_documentor_netcore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedFiltersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedFiltersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FeedFilters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedFilter>>> GetFeedFilters()
        {
            return await _context.FeedFilters.ToListAsync();
        }

        // GET: api/FeedFilters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FeedFilter>> GetFeedFilter(int id)
        {
            var feedFilter = await _context.FeedFilters.FindAsync(id);

            if (feedFilter == null)
            {
                return NotFound();
            }

            return feedFilter;
        }

        // PUT: api/FeedFilters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedFilter(int id, FeedFilter feedFilter)
        {
            if (id != feedFilter.Id)
            {
                return BadRequest();
            }

            _context.Entry(feedFilter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedFilterExists(id))
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

        // POST: api/FeedFilters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FeedFilter>> PostFeedFilter(FeedFilter feedFilter)
        {
            _context.FeedFilters.Add(feedFilter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeedFilter", new { id = feedFilter.Id }, feedFilter);
        }

        // DELETE: api/FeedFilters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FeedFilter>> DeleteFeedFilter(int id)
        {
            var feedFilter = await _context.FeedFilters.FindAsync(id);
            if (feedFilter == null)
            {
                return NotFound();
            }

            _context.FeedFilters.Remove(feedFilter);
            await _context.SaveChangesAsync();

            return feedFilter;
        }

        private bool FeedFilterExists(int id)
        {
            return _context.FeedFilters.Any(e => e.Id == id);
        }
    }
}
