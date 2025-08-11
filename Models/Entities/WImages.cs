using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Models.Entities
{
    public class WImages
    {
        public int Id { get; set; }

        public int WorkoutId { get; set; }
        [ForeignKey("WorkoutId")]
        public Workout Workout { get; set; }

        public string WorkoutImgUrl { get; set; }

        public string WorkoutName { get; set; }
    }
}
