using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        // Track the last time the user was active
        public DateTime? LastActivityDate { get; set; }

        // Track the online status (optional)
        public bool IsOnline => LastActivityDate.HasValue && LastActivityDate > DateTime.UtcNow.AddMinutes(-3);

        public string? workout_diet_plan {  get; set; }
    }
}

