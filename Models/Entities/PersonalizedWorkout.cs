using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FitnessTracker.Models.Entities
{
    public class PersonalizedWorkout
    {

        [Key]
        public int ExerciseId { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser Users { get; set; }
        [Required]
        public string ExerciseName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<PSet> Sets { get; set; }

    }
}
