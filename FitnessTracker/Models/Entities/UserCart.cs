using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Models.Entities
{
    public class UserCart
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public string UserId { get; set; }
        public Product product { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Range(1,188, ErrorMessage ="1 through 188")]
        public int Quantity { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}
