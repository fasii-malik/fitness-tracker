using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessTracker.Models
{
    public class RolesVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public List<SelectListItem> AvailableRoles { get; set; }

        public List<ApplicationUser>? usersWithCustomerRole { get; set; }
        public List<ApplicationUser>? usersWithTrainerRole { get; set; }

        public List<WorkoutCategory>? workoutCategories { get; set; }

        public string assignedPlan { get; set; }        

    }
}
