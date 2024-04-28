using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SongsAPIWebApp.Models;
using static System.Net.WebRequestMethods;

namespace SongsAPIWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly SongsAPIContext _context;

        public SongsController(SongsAPIContext context)
        {
            _context = context;
        }

        // GET: api/Songs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.ToListAsync();
        }

        // GET: api/Songs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        // PUT: api/Songs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSong(int id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            _context.Entry(song).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
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

        // POST: api/Songs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song)
        {
            var existingSong = await _context.Songs.FirstOrDefaultAsync(f => f.Name == song.Name);
            if (existingSong != null)
            {
                return BadRequest(new { message = "Пісня з такою назвою вже існує." });
            }

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSong", new { id = song.Id }, song);
        }


        // DELETE: api/Songs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }

        [HttpGet("search")] //пошук за жанром та виконавцем пісні
        public async Task<ActionResult<IEnumerable<Song>>> Search([FromQuery] int? genre, [FromQuery] int? singer)
        {
            var songs = _context.Songs.AsQueryable();

            if (genre.HasValue)
            {
                songs = songs.Where(s => s.GenreId == genre);
            }
            if (singer.HasValue)
            {
                songs = songs.Where(s => s.SingersSongs.Any(ss => ss.SingerId == singer));
            }

            return Ok(await songs.ToListAsync());
        }

        // POST: api/Songs/purchase/5
        [HttpPost("purchase/{id}")]
        public async Task<ActionResult> PurchaseSong(int id, [FromBody] Customer model)
        {
            if (model.CardNumber.Length != 16 || !model.CardNumber.All(char.IsDigit))
            {
                return BadRequest(new { message = "Номер карти має містити рівно 16 цифр." });
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            var purchase = new Purchase
            {
                SongId = id,
                CustomerId = 1,
                Status = "Куплено"
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пісня куплена." });
        }


        // GET: api/Songs/5/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<Song>> GetSongDetails(int id)
        {
            var song = await _context.Songs
                .Include(s => s.Genre)
                .Include(s => s.SingersSongs).ThenInclude(ss => ss.Singer)
                .Include(s => s.Purchases) 
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            var isPurchased = song.Purchases.Any(p => p.SongId == id); 
            return Ok(new { song = song, isPurchased = isPurchased });
        }
    }
}
