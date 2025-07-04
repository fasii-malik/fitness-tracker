using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Models.Entities
{
    public class PSet
    {
        [Key]
        public int SetId { get; set; }
        public int ExerciseId { get; set; }
        [ForeignKey("ExerciseId")]
        public int Reps { get; set; }
        public double Weight { get; set; }
        public PersonalizedWorkout Exercise { get; set; }

        public DateTime SetDateTime { get; set; } = DateTime.Now;
    }
}
