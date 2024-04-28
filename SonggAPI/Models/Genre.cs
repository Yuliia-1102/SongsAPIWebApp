using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongsAPIWebApp.Models
{
    public partial class Genre
    {
        public Genre()
        {
            Songs = new List<Song>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Display(Name = "Назва жанру")]
        public string Name { get; set; } = null!;
        public virtual ICollection<Song> Songs { get; set; }
    }
}
