using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FitnessTracker.Controllers
{
    public class WorkoutController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _HostEnvironment;

        public WorkoutController(ApplicationDbContext dbContext, IWebHostEnvironment hostEnvironment,UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _HostEnvironment = hostEnvironment;
            _userManager = userManager;
        }
        public IActionResult WorkoutIndex()
        {
            var catergoryList = _dbContext.WorkoutCategories.ToList();
            WorkoutVM workoutVM = new WorkoutVM()
            {
                WorkoutCategoriesList = catergoryList
            };
            return View(workoutVM);  
        }
        public IActionResult WorkoutList(int workoutCategoryId) 
        {
            if(workoutCategoryId != 0)
            {
                var workoutList = _dbContext.Workouts.Where(u => u.WorkoutCategoryId == workoutCategoryId).ToList();
                var catergoryList = _dbContext.WorkoutCategories.Where(u => u.Id == workoutCategoryId).ToList();
                WorkoutVM workoutVM = new WorkoutVM()
                {
                    WorkoutList = workoutList,
                    WorkoutCategoriesList = catergoryList
                };
                return View(workoutVM);
            }
            else
            {
                var workoutList = _dbContext.Workouts.ToList();
                var catergoryList = _dbContext.WorkoutCategories.ToList();
                WorkoutVM workoutVM = new WorkoutVM()
                {
                    WorkoutList = workoutList,
                    WorkoutCategoriesList = catergoryList
                };
                return View(workoutVM);
            }            
        }
        [HttpGet]
        public IActionResult WorkoutAdmin()
        {
            var workoutList = _dbContext.Workouts.Include(u=>u.WorkoutCategory).ToList();
            return View(workoutList);
        }

        [HttpGet]
        public IActionResult DietIndex()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddWorkout()
        {
            WorkoutVM workoutVM = new WorkoutVM()
            {
                WImages = new WImages(),
                workoutCategoryList = _dbContext.WorkoutCategories.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };
            
            return View(workoutVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddWorkout(WorkoutVM workoutVM)
        {
            var homeImgUrl = "";
            if (workoutVM != null && workoutVM.Images != null && workoutVM.Images.Any())
            {
                foreach (var images in workoutVM.Images)
                {
                    homeImgUrl = images.FileName;
                    if (homeImgUrl.Contains("Home"))
                    {
                        homeImgUrl = UploadFiles(images);
                        break;
                    }
                }
            }
            workoutVM.Workout.HomeImgUrl = homeImgUrl;
            await _dbContext.Workouts.AddAsync(workoutVM.Workout);
            await _dbContext.SaveChangesAsync();

            var NewProduct = await _dbContext.Workouts.Include(u => u.WorkoutCategory)
                                                      .FirstOrDefaultAsync(u => u.Name == workoutVM.Workout.Name);

            // Handle additional image uploads for PImages
            if (workoutVM.Images != null)
            {
                foreach (var image in workoutVM.Images)
                {
                    string tempFileName = image.FileName;
                    if (!tempFileName.Contains("Home"))
                    {
                        string stringFileName = UploadFiles(image);

                        // Ensure image upload succeeded
                        if (!string.IsNullOrEmpty(stringFileName))
                        {
                            var addressImage = new WImages
                            {
                                WorkoutImgUrl = stringFileName,
                                WorkoutId = NewProduct.Id,
                                WorkoutName = NewProduct.Name
                            };
                            await _dbContext.WImages.AddAsync(addressImage);
                        }
                        else
                        {
                            Console.WriteLine("File upload failed for: " + image.FileName);
                        }
                    }
                }

                // Ensure images are saved
                await _dbContext.SaveChangesAsync();
            }


            return RedirectToAction("WorkoutAdmin","Workout");
        }
        [HttpGet]
        public IActionResult EditWorkout(int id)
        {
            var workout = _dbContext.Workouts.FirstOrDefault(u => u.Id == id);
            WorkoutVM workoutVM = new WorkoutVM()
            {
                Workout = workout,
                workoutCategoryList = _dbContext.WorkoutCategories.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            workoutVM.Workout.WImgUrls = _dbContext.WImages.Where(u => u.Id == id).ToList();
            return View(workoutVM);
        }
        [HttpPost]
        public IActionResult EditWorkout(WorkoutVM workoutVM)
        {
            var workoutToEdit = _dbContext.Workouts.FirstOrDefault(u=>u.Id == workoutVM.Workout.Id);
            if(workoutToEdit != null)
            {
                workoutToEdit.Name = workoutVM.Workout.Name;
                workoutToEdit.Reps = workoutVM.Workout.Reps;
                workoutToEdit.WorkoutCategoryId = workoutVM.Workout.WorkoutCategoryId;
                workoutToEdit.PrimaryMuscleWorked = workoutVM.Workout.PrimaryMuscleWorked;
                workoutToEdit.SecondaryMuscleWorked = workoutVM.Workout.SecondaryMuscleWorked;
                workoutToEdit.Description = workoutVM.Workout.Description;

                if (workoutVM.Images != null)
                {
                    foreach (var item in workoutVM.Images)
                    {
                        string tempFileName = item.FileName;
                        if (!tempFileName.Contains("Home"))
                        {
                            string stringFileName = UploadFiles(item);
                            var addressImage = new WImages
                            {
                                WorkoutImgUrl = stringFileName,
                                WorkoutId = workoutVM.Workout.Id, // Ensure you're using the existing Product ID, not setting it manually
                                WorkoutName = workoutVM.Workout.Name
                            };
                            _dbContext.WImages.Add(addressImage);
                        }
                        else
                        {
                            if (workoutToEdit.HomeImgUrl == "")
                            {
                                string homeImgUrl = item.FileName;
                                if (homeImgUrl.Contains("Home"))
                                {
                                    homeImgUrl = UploadFiles(item);
                                    workoutToEdit.HomeImgUrl = homeImgUrl;
                                }
                            }
                        }
                    }
                }
                _dbContext.Workouts.Update(workoutToEdit);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("WorkoutAdmin", "Workout");
        }

        [HttpGet]
        public IActionResult DeleteWorkout(int Id)
        {
            var WorkoutToDelete = _dbContext.Workouts.FirstOrDefault(u => u.Id == Id);
            var ImagesTODelete = _dbContext.WImages.Where(u => u.WorkoutId == Id).Select(u => u.WorkoutImgUrl);
            foreach (var image in ImagesTODelete)
            {
                string imageUrl = "WorkoutImages\\" + image;
                var toDeleteImageFromFolder = Path.Combine(_HostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
                DeleteAImage(toDeleteImageFromFolder);
            }

            if (WorkoutToDelete.HomeImgUrl != "")
            {
                string imageUrl = "WorkoutImages\\" + WorkoutToDelete.HomeImgUrl;
                var toDeleteImageFromFolder = Path.Combine(_HostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
                DeleteAImage(toDeleteImageFromFolder);
            }
            _dbContext.Workouts.Remove(WorkoutToDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("WorkoutAdmin", "Workout");
        }
        [HttpGet]
        public async Task<IActionResult> PersonalizedWorkout()
        {
            var user = await _userManager.GetUserAsync(User);
            var pWorkoutList = _dbContext.PersonalizedWorkouts.Include(x => x.Sets).Where(u=>u.UserId == user.Id).ToList();
            PSet pSet = new PSet();

            // If the user is null, it means nobody is logged in, handle this as needed
            if (user != null)
            {
                PersonalizedWorkoutVM personalizedWorkoutVM = new PersonalizedWorkoutVM();
                personalizedWorkoutVM.users = user;
                personalizedWorkoutVM.personalizedWorkoutsList = pWorkoutList;
                personalizedWorkoutVM.PSet = pSet;
                
                return View(personalizedWorkoutVM); // Handle as per your requirement, like redirect to login
            }

            // Pass user info to the view
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PersonalizedWorkout(PersonalizedWorkoutVM personalizedWorkoutVM)
        {
            var user = await _userManager.GetUserAsync(User);
            var exerciseNames = await _dbContext.PersonalizedWorkouts
                                    .Select(pw => pw.ExerciseName )
                                    .ToListAsync();
            var workoutList = _dbContext.PersonalizedWorkouts.ToList();

            if(personalizedWorkoutVM.PSet != null)
            {
               await _dbContext.PSets.AddAsync(personalizedWorkoutVM.PSet);
               await _dbContext.SaveChangesAsync();
            }

          //This a Validation so same name exercise cannot be added again
            //foreach (var items in workoutList) { 
            //    if (items.ExerciseName.Contains(personalizedWorkoutVM.personalizedWorkout.ExerciseName) && items.UserId.Contains(user.Id))
            //    {
            //        return BadRequest("Exercise name already exists.");
            //    }
            //}
            if(personalizedWorkoutVM.personalizedWorkout != null && user != null)
            {
               await _dbContext.PersonalizedWorkouts.AddAsync(personalizedWorkoutVM.personalizedWorkout);
               await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("PersonalizedWorkout", "Workout");
    
        }
        public IActionResult DeletePersonalizedWorkout(int id)
        {
            var pWorkoutToRemove = _dbContext.PersonalizedWorkouts.Include(p => p.Sets).FirstOrDefault(u => u.ExerciseId == id);
            if(pWorkoutToRemove != null)
            {
                _dbContext.PersonalizedWorkouts.Remove(pWorkoutToRemove);
                _dbContext.SaveChanges();
            }
            
            return RedirectToAction("PersonalizedWorkout", "Workout");
        }
        public IActionResult DeleteSet(int setId)
        {
            var setToDelete = _dbContext.PSets.FirstOrDefault(u => u.SetId == setId);
            if (setToDelete != null) { 
                _dbContext.PSets.Remove(setToDelete);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("PersonalizedWorkout", "Workout");
        }
        private void DeleteAImage(string toDeleteImageFromFolder)
        {
            if (System.IO.File.Exists(toDeleteImageFromFolder))
            {
                System.IO.File.Delete(toDeleteImageFromFolder);
            }
        }

        private string UploadFiles(IFormFile image)
        {
            string fileName = null;
            if (image != null)
            {
                string uploadDirLocation = Path.Combine(_HostEnvironment.WebRootPath, "WorkoutImages");//Going to wwwroot/Images folder
                fileName = Guid.NewGuid().ToString() + "_" + image.FileName;//combining Guid with images name
                string filePath = Path.Combine(uploadDirLocation, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return fileName;
        }
    }//class end
}//namespace end
