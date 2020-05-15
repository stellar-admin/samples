using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Product
    {
        [Required]
        public Category Category { get; set; }

        public int CategoryId { get; set; }

        public string Description { get; set; }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}