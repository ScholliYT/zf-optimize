using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.Data.Entities
{
    public class Product
    {
        [Key] public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }
    }
}