using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongsAPIWebApp.Models
{
    public class Customer
    {
        public Customer()
        {
            Purchases = new List<Purchase>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(40)")]
        [Display(Name = "Ім'я")]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(16)")]
        [Display(Name = "Номер картки")]
        public string CardNumber { get; set; } = null!;
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
