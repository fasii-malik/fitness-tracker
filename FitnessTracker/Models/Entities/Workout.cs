using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models.Entities
{
    public class Workout
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Reps { get; set; }

        [Required]
        [MaxLength(2000, ErrorMessage = "Length cannot exceede more than 30 characters!")]
        public string Description { get; set; }

        public ICollection<WImages>? WImgUrls { get; set; }
        [Required]

        public int WorkoutCategoryId { get; set; }

        [ForeignKey("WorkoutCategoryId")]

        public WorkoutCategory? WorkoutCategory { get; set; }

        public string? HomeImgUrl { get; set; }

        public string? SecondaryMuscleWorked { get; set; }
        [Required]
        public string PrimaryMuscleWorked { get; set; }
    }
}
