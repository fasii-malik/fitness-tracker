using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models.Entities
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$" , ErrorMessage ="Please Insert Price Like This: 0.00")]
        public double Purchase_Price { get; set; }

        public string Category { get; set; }

        public int Quantity { get; set; }
    }
}
