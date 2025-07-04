using FitnessTracker.Data;
using FitnessTracker.Migrations;
using FitnessTracker.Models;
using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace FitnessTracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await dbContext.categories.ToListAsync();
            return View(categories);
        }
        public IActionResult AddCategory()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDto _category)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid category data.";
                return BadRequest();                
            }

            // Create a new Category entity using the data from _category DTO
            var category = new FitnessTracker.Models.Entities.Category
            {
                Name = _category.Name // Map the Name from the DTO to the entity
            };

            // Add the category to the DbContext
            dbContext.categories.Add(category);           
            await dbContext.SaveChangesAsync();
            TempData["success"] = category.Name + " Succesfully Added!";
            // Optionally redirect to a list of categories or return a success message
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await dbContext.categories.FindAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(FitnessTracker.Models.Entities.Category viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with the invalid model to display errors
                return View(viewModel);
            }
            var category = await dbContext.categories.FindAsync(viewModel.Id);
            if (category is not null)
            {
                category.Name = viewModel.Name;

                await dbContext.SaveChangesAsync();
                TempData["success"] = category.Name + " Succesfully Edited!";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(FitnessTracker.Models.Entities.Category viewModel)
        {
            var customer = await dbContext.categories.FindAsync(viewModel.Id);
            if (customer is not null)
            {
                dbContext.categories.Remove(customer);
                await dbContext.SaveChangesAsync();
                TempData["success"] = "Successfully Deleted!";
            }
            
            return RedirectToAction("Index");
        }
    }//controller end
    
}//namesapce end
