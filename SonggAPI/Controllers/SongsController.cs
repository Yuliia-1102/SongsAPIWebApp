using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Humanizer.Localisation;
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
                return BadRequest(new { message = "Не знайдено пісню з таким індексом." });
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
                return BadRequest(new { message = "Неправильний ідентифікатор пісні." });
            }

            if (!_context.Genres.Any(g => g.Id == song.GenreId))
            {
                return BadRequest(new { message = "Жанр з вказаним індексом не існує в базі даних." });
            }

            var existingName = await _context.Songs
                .FirstOrDefaultAsync(s => s.Name.ToLower() == song.Name.ToLower() && s.Id != id);

            if (existingName != null)
            {
                return BadRequest(new { message = "Пісня з такою назвою вже існує." });
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
                    return BadRequest(new { message = "Не знайдено пісню з таким індексом." });
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
            var existingSong = await _context.Songs.FirstOrDefaultAsync(f => f.Name.ToLower() == song.Name.ToLower());
            if (existingSong != null)
            {
                return BadRequest(new { message = "Пісня з такою назвою вже існує." });
            }

            if (!_context.Genres.Any(g => g.Id == song.GenreId))
            {
                return BadRequest(new { message = "Жанр з вказаним індексом не існує в базі даних." });
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
                return BadRequest(new { message = "Неправильний ідентифікатор пісні." });
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Song>>> Search([FromQuery] int? genre, [FromQuery] int? singer) 
        {
            if (genre.HasValue && !_context.Genres.Any(g => g.Id == genre.Value))
            {
                return NotFound(new { message = "Жанру з таким ідентифікатором не знайдено." });
            }

            if (singer.HasValue && !_context.Singers.Any(s => s.Id == singer.Value))
            {
                return NotFound(new { message = "Виконавця з таким ідентифікатором не знайдено." });
            }

            var songs = _context.Songs.AsQueryable();

            if (genre.HasValue)
            {
                songs = songs.Where(s => s.GenreId == genre.Value); // фільтрація
            }
            if (singer.HasValue)
            {
                songs = songs.Where(s => s.SingersSongs.Any(ss => ss.SingerId == singer.Value)); // фільтрація
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

            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
            {
                return BadRequest(new { message = "Неправильний ідентифікатор пісні." });
            }

            // Перевірка, чи пісня вже була куплена 
            var existingPurchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.SongId == id && p.CustomerId == 1);
            if (existingPurchase != null)
            {
                return BadRequest(new { message = "Пісня вже куплена." });
            }

            var purchase = new Purchase
            {
                SongId = id,
                CustomerId = 1, 
                Status = "Куплено"
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пісня куплена.", audioUrl = song.AudioUrl });
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
                return BadRequest(new { message = "Неправильний ідентифікатор пісні." });
            }

            var isPurchased = song.Purchases.Any(p => p.SongId == id); 
            return Ok(new { song = song, isPurchased = isPurchased });
        }
    }
}
