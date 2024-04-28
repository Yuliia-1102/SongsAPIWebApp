using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongsAPIWebApp.Models
{
    public class Purchase
    {
        public int SongId { get; set; }
        public int CustomerId { get; set; }

        [Display(Name = "Статус пісні")]
        [Column(TypeName = "nvarchar(20)")]
        public string? Status { get; set; }

        [Display(Name = "Покупець")]
        public virtual Customer Customer { get; set; } = null!;

        [Display(Name = "Пісня")]
        public virtual Song Song { get; set; } = null!;
    }
}
