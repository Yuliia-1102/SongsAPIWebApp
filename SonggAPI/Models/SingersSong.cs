using System.ComponentModel.DataAnnotations;

namespace SongsAPIWebApp.Models
{
    public partial class SingersSong
    {
        [Display(Name = "Пісня")]
        public int SongId { get; set; }

        [Display(Name = "Виконавець")]
        public int SingerId { get; set; }

        [Display(Name = "Виконавець")]
        public virtual Singer Singer { get; set; } = null!;

        [Display(Name = "Пісня")]
        public virtual Song Song { get; set; } = null!;
    }
}
