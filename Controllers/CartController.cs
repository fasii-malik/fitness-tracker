using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CartController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize]
        public IActionResult CartIndex()
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                CartIndexVM cartIndexVM = new CartIndexVM()
                {
                    productList = _dbContext.userCart.Include(u => u.product).Where(u => u.UserId.Contains(userId)).ToList(),
                };
                return View(cartIndexVM);
            }
            return BadRequest();
        }

        public IActionResult MinusAnItem(int productId)
        {
            var itemToMinus = _dbContext.userCart.FirstOrDefault(u => u.ProductId == productId);
            if (itemToMinus != null)
            {
                if (itemToMinus.Quantity - 1 == 0)
                {
                    _dbContext.userCart.Remove(itemToMinus);
                }
                else
                {
                    itemToMinus.Quantity -= 1;
                    _dbContext.userCart.Update(itemToMinus);
                }
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(CartIndex));
        }

        public IActionResult DeleteAnItem(int productId)
        {
            var itemToRemove = _dbContext.userCart.FirstOrDefault(u => u.ProductId == productId);
            if (itemToRemove != null)
            {

                _dbContext.userCart.Remove(itemToRemove);

                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(CartIndex));
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, string? returnUrl)
        {
            // Check if product exists in the Products table
            var productAddToCart = await _dbContext.Products.FirstOrDefaultAsync(u => u.Id == productId);
            if (productAddToCart == null)
            {
                // If the product doesn't exist, return an error or redirect
                return RedirectToAction("Shop", "Home", new { error = "Product not found" });
            }

            var CheckIfUserSignedInOrNot = _signInManager.IsSignedIn(User);
            if (CheckIfUserSignedInOrNot)
            {
                var user = _userManager.GetUserId(User);
                if (user != null)
                {
                    var getTheCartIfAnyExistForTheUser = await _dbContext.userCart.Where(u => u.UserId == user).ToListAsync();

                    if (getTheCartIfAnyExistForTheUser.Count() > 0)
                    {
                        var getTheQuantity = getTheCartIfAnyExistForTheUser.FirstOrDefault(p => p.ProductId == productId);
                        if (getTheQuantity != null)
                        {
                            getTheQuantity.Quantity = getTheQuantity.Quantity + 1;
                            _dbContext.userCart.Update(getTheQuantity);
                        }
                        else
                        {
                            // Adding a new product to the cart
                            UserCart newItemToCart = new UserCart
                            {
                                ProductId = productId,
                                UserId = user,
                                Quantity = 1
                            };
                            await _dbContext.userCart.AddAsync(newItemToCart);
                        }
                    }
                    else
                    {
                        // Cart is empty, adding first product
                        UserCart newItemToCart = new UserCart
                        {
                            ProductId = productId,
                            UserId = user,
                            Quantity = 1
                        };
                        await _dbContext.userCart.AddAsync(newItemToCart);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                if (returnUrl != null)
                {
                    return RedirectToAction("CartIndex", "Cart");
                }
            }
            return RedirectToAction("Shop", "Home");
        }
    }
    }
