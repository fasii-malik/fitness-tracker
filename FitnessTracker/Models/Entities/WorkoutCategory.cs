using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models.Entities
{
    public class WorkoutCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Length can't go above 30")]
        public string Name { get; set; }
    }
}
