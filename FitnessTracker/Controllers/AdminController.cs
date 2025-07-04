using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers
{
    
    // [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext dbContext, SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            var usersCount = _userManager.Users.ToList().Count();
            var totalSales = _dbContext.OrderHeader.Select(t => t.TotalOrderAmount).Sum();
            ViewBag.UsersCount = usersCount;
            ViewBag.TotalSales = totalSales;
            return View();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Customers()
        {

            //var users = await dbContext.Registration.ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> EditCustomer(string id)
        {
            // Retrieve the customer by ID
            // var customer = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(ApplicationUser viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Edit User Failed!";
                // Return the view with the invalid model to display errors
                return View(viewModel);
            }

            // Find the existing customer
            var customer = await _userManager.FindByIdAsync(viewModel.Id.ToString());
            if (customer == null)
            {                
                return NotFound();
            }

            // Update customer details
            customer.FirstName = viewModel.FirstName;
            customer.LastName = viewModel.LastName;
            customer.Email = viewModel.Email;

            // If you need to update the password, it should be handled separately
            // var result = await _userManager.ChangePasswordAsync(customer, viewModel.CurrentPassword, viewModel.NewPassword);
            // if (!result.Succeeded)
            // {
            //     // Handle the password update error
            // }

            var updateResult = await _userManager.UpdateAsync(customer);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(viewModel); // Return to the view with validation errors
            }
            TempData["success"] = "Successfully Edited!";
            return RedirectToAction("Customers", "Admin");
        }


        public async Task<IActionResult> Delete(ApplicationUser viewModel)
        {
            //var customer = await dbContext.Registration.FindAsync(viewModel.Id);
            //if (customer is not null)
            //{
            //    dbContext.Registration.Remove(customer);
            //    await dbContext.SaveChangesAsync();
            //}
            var customerToDelete = await _userManager.FindByIdAsync(viewModel.Id.ToString());
            if (customerToDelete != null)
            {
                await _userManager.DeleteAsync(customerToDelete);
                TempData["success"] = "Successfully Deleted!";
            }
            else
            {
                TempData["error"] = "Failed to Delete!";
                return RedirectToAction("Customers", "Admin");
            }            
            return RedirectToAction("Customers", "Admin");
        }

        public async Task<IActionResult> RolesIndex()
        {
            var users = _userManager.Users.ToList();
            var rolesList = _roleManager.Roles.ToList();
            var userRolesViewModel = new List<RolesVM>();

            foreach (var user in users)
            {
                var thisViewModel = new RolesVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,                    
                    Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                    AvailableRoles = rolesList.Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList() // Populate available roles for dropdown
                };

                userRolesViewModel.Add(thisViewModel);
            }

            return View(userRolesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string UserId, string RoleName)
        {
            // Check if both UserId and RoleName are provided
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(RoleName))
            {
                // Handle the case when either is missing
                TempData["error"] = "User or role is missing!";
                return RedirectToAction("RolesIndex");
            }

            // Retrieve the user by their Id
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                // Handle the case when the user is not found
                TempData["error"] = "User not Found!";
                return RedirectToAction("RolesIndex");
            }

            // Check if the role exists
            var roleExists = await _roleManager.RoleExistsAsync(RoleName);
            if (!roleExists)
            {
                // Handle the case when the role does not exist
                TempData["error"] = "role is missing!";
                return RedirectToAction("RolesIndex");
            }

            // Get all roles assigned to the user
            var currentRoles = await _userManager.GetRolesAsync(user);

            if(user.UserName == "fasii@gmail.com" && currentRoles.Contains("Admin"))
            {
                TempData["error"] = "Cannot Change Admin Role!";
                return RedirectToAction("RolesIndex");
            }

            // Remove all current roles (ensure only one role is assigned)
            if (currentRoles.Any())
            {
                var resultRemove = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!resultRemove.Succeeded)
                {
                    // Handle failure when removing roles
                    TempData["error"] = "Error removing current role!";
                    return RedirectToAction("RolesIndex");
                }
            }

            // Assign the new role to the user
            var result = await _userManager.AddToRoleAsync(user, RoleName);

            if (result.Succeeded)
            {
                // Success - redirect to the RolesIndex with a success message
                TempData["success"] = "Role Assigned Successfully!";
                return RedirectToAction("RolesIndex", new { message = "Role assigned successfully" });
            }

            // Handle failure case (e.g., if there was an issue assigning the role)
            return RedirectToAction("RolesIndex", new { message = "Error assigning role" });
        }
        [HttpGet]
        public async Task<IActionResult> TrainerIndex()
        {
                var roleExists = await _roleManager.RoleExistsAsync("Customer");
            var rolesList = _roleManager.Roles.ToList();
            if (!roleExists)
            {
                return NotFound("Customer role not found.");
            }

            // Get all users in the "Customer" role
            var usersInCustomerRole = (await _userManager.GetUsersInRoleAsync("Customer")).ToList();
            var usersInTrainerRole = (await _userManager.GetUsersInRoleAsync("Trainer")).ToList();
            var workoutCategories = _dbContext.WorkoutCategories.ToList();
            RolesVM rolesVM = new RolesVM()
            {
                usersWithCustomerRole = usersInCustomerRole,
                usersWithTrainerRole = usersInTrainerRole,
                workoutCategories = workoutCategories,
                AvailableRoles = workoutCategories.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };
            return View(rolesVM);
        }

        [HttpPost]
        public async Task<IActionResult> TrainerIndex(RolesVM rolesVM)
        {
            
            var user = await _userManager.FindByIdAsync(rolesVM.UserId);

            if (user != null) 
            {
                user.workout_diet_plan = rolesVM.assignedPlan;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            
            return RedirectToAction("TrainerIndex","Admin");
        }

        public async Task<IActionResult> AddRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                TempData["error"] = "Role Name is Null!";
                return RedirectToAction("RolesIndex", "Admin");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                TempData["error"] = "Role Already Exists!";
                return RedirectToAction("RolesIndex", "Admin");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                TempData["success"] = "Role Successfully Added!";
                return RedirectToAction("RolesIndex","Admin");
            }
            else
            {
                TempData["error"] = "Error Creating Role!";
                return RedirectToAction("RolesIndex", "Admin");
            }
        }
        public async Task<IActionResult> DeleteRole(string roleName)
        {      
            var roleToDelete = await _roleManager.FindByNameAsync(roleName);
            if (roleToDelete.Name.Contains("Admin")) {
                TempData["error"] = "Admin Cannot be Deleted!";
                return RedirectToAction("RolesIndex");                
            }
            if (roleToDelete == null)
            {
                return NotFound("Role not found");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (result.Succeeded)
            {
                TempData["success"] = "Role Successfully Deleted!";
                return RedirectToAction("RolesIndex");
            }

            return View("RolesIndex");
        }

            public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();

            // Redirect to the login page or home page after logging out
            return RedirectToAction("Index", "Home");
        }
    }
}
