using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongsAPIWebApp.Models
{
    public partial class Singer
    {
        public Singer()
        {
            SingersSongs = new List<SingersSong>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(40)")]
        [Display(Name = "Ім'я")]
        public string Name { get; set; } = null!;

        public virtual ICollection<SingersSong> SingersSongs { get; set; }
    }
}
