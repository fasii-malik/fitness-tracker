using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessTracker.Models
{
    public class WorkoutVM
    {
        public Workout? Workout {  get; set; }
        public IEnumerable<IFormFile> Images { get; set; }
        public WImages? WImages { get; set; }
    public IEnumerable<SelectListItem> workoutCategoryList { get; set; }

        public List<Workout> WorkoutList { get; set; }
        public List<WorkoutCategory> WorkoutCategoriesList { get; set; }
    }
}
