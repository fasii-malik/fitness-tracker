using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Please Insert Price Like This: 0.00")]
        public double Price { get; set; }

        [Required]
        [MaxLength(2000, ErrorMessage = "Length cannot exceede more than 30 characters!")]
        public string Description { get; set; }

        public ICollection<PImages>? ImgUrls { get; set; }
        [Required]

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]

        public Category? Category { get; set; }

        public string? HomeImgUrl { get; set; }

    }
}
