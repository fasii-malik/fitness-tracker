using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }

        public DbSet<Category> categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<PImages> PImages { get; set; }

        public DbSet<UserCart> userCart { get; set; }

        public DbSet<UserOrderHeader> OrderHeader { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<WorkoutCategory> WorkoutCategories { get; set; }

        public DbSet<WImages> WImages { get; set; }

        public DbSet<Workout> Workouts { get; set; }

        public DbSet<PersonalizedWorkout> PersonalizedWorkouts { get; set; }

        public DbSet<PSet> PSets { get; set; }


		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	base.OnModelCreating(modelBuilder);

		//	// Seed data for WorkoutCategory
		//	modelBuilder.Entity<WorkoutCategory>().HasData(
		//		new WorkoutCategory {  Id = 1, Name = "Strength" },
		//		new WorkoutCategory {  Id = 2, Name = "Endurance" },
		//		new WorkoutCategory {  Id = 3, Name = "Cardio" },
		//		new WorkoutCategory {  Id = 4, Name = "Flexibility" }
		//	);            
		//}
	}
}
