using FitnessTracker.Data;
using FitnessTracker.Infrastructure;
using FitnessTracker.Models;
using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Stripe.Checkout;

namespace FitnessTracker.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public OrderController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult OrderDetailPreview()
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var currentUser = _dbContext.applicationUsers.FirstOrDefault(x => x.Id == userId);
                SummaryVM summaryVM = new SummaryVM()
                {
                    userCartList = _dbContext.userCart.Include(u=>u.product).Where(u=>u.UserId.Contains(userId)).ToList(),
                    orderSummary = new UserOrderHeader(),
                    cartUserId = userId,
                };
                if(currentUser != null)
                {
                    //Dont have address in user details which is created with Identity Package
                    //summaryVM.orderSummary.DeliveryStreetAddress = currentUser;
                    summaryVM.orderSummary.Name = currentUser.FirstName + " " + currentUser.LastName;
                }
                var count = _dbContext.userCart.Where(u => u.UserId.Contains(_userManager.GetUserId(User))).ToList().Count;
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);
                return View(summaryVM);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Summary(SummaryVM summaryVMFromView)
        {

            var isAuthenticated = User.Identity.IsAuthenticated;
            var username = User.Identity.Name;
            var claims = User.Claims.ToList();


            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var currentUser = _dbContext.applicationUsers.FirstOrDefault(x => x.Id == userId);
                SummaryVM summaryVM = new SummaryVM()
                {
                    userCartList = _dbContext.userCart.Include(u => u.product).Where(u => u.UserId.Contains(userId)).ToList(),
                    orderSummary = new UserOrderHeader(),
                };
                if (currentUser != null)
                {
                    summaryVM.orderSummary.Name = currentUser.FirstName + " " + currentUser.LastName;
                    summaryVM.orderSummary.DeliveryStreetAddress = summaryVMFromView.orderSummary.DeliveryStreetAddress;
                    summaryVM.orderSummary.City = summaryVMFromView.orderSummary.City;
                    summaryVM.orderSummary.PostalCode = summaryVMFromView.orderSummary.PostalCode;
                    summaryVM.orderSummary.PhoneNumber = summaryVMFromView.orderSummary.PhoneNumber;
                    summaryVM.orderSummary.TotalOrderAmount = summaryVMFromView.orderSummary.TotalOrderAmount;
                    summaryVM.orderSummary.DateOfOrder = DateTime.Now;
                    summaryVM.orderSummary.OrderStatus = "Pending";
                    summaryVM.orderSummary.PaymentStatus = "Not Paid";
                    summaryVM.orderSummary.UserId = summaryVMFromView.cartUserId;
                    
                    await _dbContext.AddAsync(summaryVM.orderSummary);
                    await _dbContext.SaveChangesAsync();
                }

                var totalAmount = summaryVMFromView.orderSummary.TotalOrderAmount;
                Console.WriteLine($"TotalOrderAmount: {totalAmount}");

                if (summaryVMFromView.orderSummary.TotalOrderAmount > 0)
                {
                    var options = new SessionCreateOptions
                    {
                        SuccessUrl = Url.Action("OrderSuccess", "Order", new { id = summaryVM.orderSummary.Id }, protocol: Request.Scheme),
                        CancelUrl = Url.Action("OrderCancel", "Order", new { id = summaryVM.orderSummary.Id }, protocol: Request.Scheme),
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment"
                    };

                    foreach (var item in summaryVM.userCartList)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions{
                                UnitAmount = (long) item.product.Price * 100,
                                Currency = "pkr",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.product.Name,
                                    Description = item.product.Description
                                }
                            },
                            Quantity = item.Quantity,
                        };
                        options.LineItems.Add(sessionLineItem);
                    }
                    var service = new SessionService();
                    Session session = service.Create(options);
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);

                }
                else
                {
                    return View();
                }
            }
            return RedirectToAction("Shop","Home");
        }

        public IActionResult OrderCancel(int id)
        {
            var orderProcessCanceled = _dbContext.OrderHeader.FirstOrDefault(u => u.Id == id);
            _dbContext.OrderHeader.Remove(orderProcessCanceled);
            _dbContext.SaveChanges();
            return RedirectToAction("CartIndex", "Cart");
        }

        public IActionResult OrderSuccess(int id)
        {

                var userId = _userManager.GetUserId(User);
                var userCartRemove = _dbContext.userCart.Where(u=>u.UserId.Contains(userId)).ToList();
                var orderProcessed = _dbContext.OrderHeader.FirstOrDefault(h => h.Id == id);
                if(orderProcessed != null){
                    if(orderProcessed.PaymentStatus == "Not Paid")
                    {
                        orderProcessed.PaymentStatus = "Paid";
                    orderProcessed.PaymentProcessDate = DateTime.Now;
                    }
                }
                foreach (var list in userCartRemove)
                {
                    OrderDetails orderReceived = new OrderDetails()
                    {
                        OrderHeaderId = orderProcessed.Id,
                        ProductId = (int)list.ProductId,
                        Count = list.Quantity
                    };
                    _dbContext.OrderDetails.Add(orderReceived);
                }
            ViewBag.OrderId = id;
                _dbContext.userCart.RemoveRange(userCartRemove);
                _dbContext.SaveChanges();
                var count = _dbContext.userCart.Where(u=>u.UserId.Contains(userId)).ToList().Count;
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);
            return View();
        }

        public IActionResult OrderHistory(String? status)
        {
            var userId = _userManager.GetUserId(User);
            List<UserOrderHeader> orderList = new List<UserOrderHeader>();
            if(status != null && status != "All")
            {
                if(User.IsInRole("Admin")){
                    orderList = _dbContext.OrderHeader.Where(u=>u.OrderStatus == status).ToList();
                }
                else
                {
                    orderList = _dbContext.OrderHeader.Where(u => u.OrderStatus == status && u.UserId == userId).ToList();

                }
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    orderList = _dbContext.OrderHeader.ToList();
                }
                else
                {
                    orderList = _dbContext.OrderHeader.Where(u=>u.UserId == userId).ToList();

                }
            }
            return View(orderList);
        }
    }
}
