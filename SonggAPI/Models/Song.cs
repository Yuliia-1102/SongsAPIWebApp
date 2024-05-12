using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongsAPIWebApp.Models
{
    public class Song
    {
        public Song()
        {
            SingersSongs = new List<SingersSong>();
            Purchases = new List<Purchase>();
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Жанр")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім.")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Назва пісні")]
        public string Name { get; set; } = null!;

        private float price;

        [Required(ErrorMessage = "Поле 'Ціна' не повинно бути порожнім.")]
        [Column(TypeName = "real")]
        [Display(Name = "Ціна")]
        [Range(0.99, float.MaxValue, ErrorMessage = "Ціна має бути більшою або дорівнювати 0.99")]
        public float Price
        {
            get { return price; }
            set { price = value; }
        }

        [Column(TypeName = "nvarchar(200)")]
        [Display(Name = "URL аудіо файлу")]
        [RegularExpression(@"^https:\/\/.*", ErrorMessage = "Посилання повинно починатися з https://")]
        public string? AudioUrl { get; set; }

        [Display(Name = "Виконавці")]
        public virtual ICollection<SingersSong> SingersSongs { get; set; }

        [Display(Name = "Жанр")]
        public virtual Genre? Genre { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
