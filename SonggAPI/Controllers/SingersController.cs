using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SongsAPIWebApp.Models;

namespace SongsAPIWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingersController : ControllerBase
    {
        private readonly SongsAPIContext _context;

        public SingersController(SongsAPIContext context)
        {
            _context = context;
        }

        // GET: api/Singers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Singer>>> GetSingers()
        {
            return await _context.Singers.ToListAsync();
        }

        // GET: api/Singers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Singer>> GetSinger(int id)
        {
            var singer = await _context.Singers.FindAsync(id);

            if (singer == null)
            {
                return NotFound();
            }

            return singer;
        }

        // PUT: api/Singers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSinger(int id, Singer singer)
        {
            if (id != singer.Id)
            {
                return BadRequest();
            }

            _context.Entry(singer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SingerExists(id))
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

        // POST: api/Singers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Singer>> PostSinger(Singer singer)
        {
            _context.Singers.Add(singer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSinger", new { id = singer.Id }, singer);
        }

        // DELETE: api/Singers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSinger(int id)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer == null)
            {
                return NotFound();
            }

            _context.Singers.Remove(singer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SingerExists(int id)
        {
            return _context.Singers.Any(e => e.Id == id);
        }
    }
}
