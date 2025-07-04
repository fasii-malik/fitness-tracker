using FitnessTracker.Models.Entities;

namespace FitnessTracker.Models
{
    public class PersonalizedWorkoutVM
    {
        public ApplicationUser users { get; set; }

        public PersonalizedWorkout? personalizedWorkout { get; set; }

        public List<PersonalizedWorkout> personalizedWorkoutsList { get; set; }

        public PSet PSet { get; set; }
        public ICollection<PSet> Sets { get; set; }
    }
}
