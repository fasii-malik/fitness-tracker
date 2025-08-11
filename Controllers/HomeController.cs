using FitnessTracker.Data;
using FitnessTracker.Infrastructure;
using FitnessTracker.Models;
using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FitnessTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext dbContext;
        //private readonly TokenProvider tokenProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext,
             UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            this.dbContext = dbContext;
           //this.tokenProvider = tokenProvider;
            _userManager = userManager;
            _signInManager = signInManager;
          //_emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //public IActionResult Login(LoginDTO loginDTO)
        //{

        //    var user = dbContext.Registration.FirstOrDefault(e => e.Email == loginDTO.Email && e.Password == loginDTO.Password);
        //    if (user != null)
        //    {
        //        string token = tokenProvider.Create(user);
        //        var cookieOptions = new CookieOptions
        //        {
        //            HttpOnly = true, // Prevent access via JavaScript
        //            Secure = true, // Ensures the cookie is only sent over HTTPS
        //            Expires = DateTime.UtcNow.AddMinutes(60)
        //        };
        //        Response.Cookies.Append("AuthToken", token, cookieOptions);
        //        return RedirectToAction("Customers", "Admin");

        //    }
        //    return Unauthorized();
        //}
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Failed to Login!";            
                return View(loginDto);
            }

            // Check user credentials and sign them in
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (User.IsInRole("Admin"))
                {                    
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {                    
                    //await _emailService.SendEmailAsync("malikfasih79@gmail.com", "Welcome!", "<h1>Hello, welcome to our app!</h1>");   
                    return RedirectToAction("Index", "Home");
                     
                }                
            }
            else
            {
                TempData["error"] = "Failed to Login!";
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(loginDto);
            }
        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(AddSignUpViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Failed to Register!";
                return BadRequest(ModelState);
            }

            var objUser = await _userManager.FindByEmailAsync(viewModel.Email); // Using UserManager to find by email
            if (objUser == null)
            {
                var user = new ApplicationUser
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    UserName = viewModel.Email, // Make sure to assign UserName, it's required by Identity
                };

                var Registration = await _userManager.CreateAsync(user, viewModel.Password);
                if (Registration.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["success"] = "Successfully Registered!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Failed to Register!";
                    // Log or return the detailed errors
                    var errors = string.Join(", ", Registration.Errors.Select(e => e.Description));
                    return BadRequest($"Failed Registration: {errors}");
                }
            }
            else
            {
                TempData["error"] = "Failed to Register User Already exists";
                return RedirectToAction("Signup");
            }
        }
        [HttpGet]
        public IActionResult Shop(string? searchByName, string? searchByCategory)
        {

            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var count = dbContext.userCart.Where(u => u.UserId.Contains(userId)).Count();
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);
            }

            ShopVM shopVM = new ShopVM();
            if(searchByName != null)
            {
                shopVM.ProductList = dbContext.Products.Where(productName => EF.Functions.Like(productName.Name, $"%{searchByName}%")).ToList();
                shopVM.CategoryList = dbContext.categories.ToList();
            }
            else if(searchByCategory != null)
            {
                var searchByCategoryName = dbContext.categories.FirstOrDefault(u => u.Name == searchByCategory);
                shopVM.ProductList = dbContext.Products.Where(u=>u.CategoryId == searchByCategoryName.Id).ToList();
                shopVM.CategoryList = dbContext.categories.Where(u => u.Name.Contains(searchByCategory));
            }
            else
            {
                shopVM.ProductList = dbContext.Products.ToList();
                shopVM.CategoryList = dbContext.categories.ToList();
            }
            

            return View(shopVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
