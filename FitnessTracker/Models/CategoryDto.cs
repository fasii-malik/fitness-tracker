using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    public class CategoryDto
    {

        [Required]
        [StringLength(30, ErrorMessage = "Length can't go above 30")]
        public string Name { get; set; }
    }
}
