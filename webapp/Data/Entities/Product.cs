using System.ComponentModel.DataAnnotations;

namespace webapp.Data.Entities
{
    public class Product
    {
        [Key] public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}