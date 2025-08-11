using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FitnessTracker.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _HostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _dbContext = context;
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _dbContext.Products.Include(u=>u.Category).ToList();
            return View(products);
        }
        [HttpGet]
        public IActionResult Add()
        {
            ProductVM productVM = new ProductVM()
            {
                Inventories = new Inventory(),
                PImages = new PImages(),
                CategoriesList = _dbContext.categories.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductVM productVM)
        {
            // Handle Home Image Upload
            string homeImgUrl = "";
            if (productVM != null && productVM.Images != null && productVM.Images.Any())
            {
                foreach (var image in productVM.Images)
                {
                    homeImgUrl = image.FileName;
                    if (homeImgUrl.Contains("Home"))
                    {
                        homeImgUrl = UploadFiles(image);
                        break;
                    }
                }
            }

            // Save product with home image URL
            productVM.Products.HomeImgUrl = homeImgUrl;
            await _dbContext.Products.AddAsync(productVM.Products);
            await _dbContext.SaveChangesAsync();

            // Fetch the newly added product
            var NewProduct = await _dbContext.Products.Include(u => u.Category)
                                                      .FirstOrDefaultAsync(u => u.Name == productVM.Products.Name);

            // Check if the product was added successfully
            if (NewProduct == null)
            {
                return BadRequest("Product not found.");
            }

            // Add inventory
            productVM.Inventories.Name = NewProduct.Name;
            productVM.Inventories.Category = NewProduct.Category.Name;
            await _dbContext.Inventories.AddAsync(productVM.Inventories);
            await _dbContext.SaveChangesAsync();

            // Handle additional image uploads for PImages
            if (productVM.Images != null)
            {
                foreach (var image in productVM.Images)
                {
                    string tempFileName = image.FileName;
                    if (!tempFileName.Contains("Home"))
                    {
                        string stringFileName = UploadFiles(image);

                        // Ensure image upload succeeded
                        if (!string.IsNullOrEmpty(stringFileName))
                        {
                            var addressImage = new PImages
                            {
                                ImgUrl = stringFileName,
                                ProductId = NewProduct.Id,
                                ProductName = NewProduct.Name
                            };
                            await _dbContext.PImages.AddAsync(addressImage);
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

            // Redirect after success
            return RedirectToAction("Index", "Product");
        }


        private string UploadFiles(IFormFile image)
        {
            string fileName = null;
            if (image != null)
            {
                string uploadDirLocation = Path.Combine(_HostEnvironment.WebRootPath, "Images");//Going to wwwroot/Images folder
                fileName = Guid.NewGuid().ToString() + "_" + image.FileName;//combining Guid with images name
                string filePath = Path.Combine(uploadDirLocation, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return fileName;
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ProductVM productVM = new ProductVM()
            {
                Products = _dbContext.Products.FirstOrDefault(p => p.Id == Id),
                CategoriesList = _dbContext.categories.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            productVM.Products.ImgUrls = _dbContext.PImages.Where(u=>u.Id == Id).ToList();
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM)
        {
            var ProductToEdit = _dbContext.Products.FirstOrDefault(u => u.Id == productVM.Products.Id);
            if (ProductToEdit != null)
            {
                ProductToEdit.Name = productVM.Products.Name;
                ProductToEdit.Price = productVM.Products.Price;
                ProductToEdit.Description = productVM.Products.Description;
                ProductToEdit.CategoryId = productVM.Products.CategoryId;

                if (productVM.Images != null)
                {
                    foreach (var item in productVM.Images)
                    {
                        string tempFileName = item.FileName;
                        if (!tempFileName.Contains("Home"))
                        {
                            string stringFileName = UploadFiles(item);
                            var addressImage = new PImages
                            {
                                ImgUrl = stringFileName,
                                ProductId = productVM.Products.Id, // Ensure you're using the existing Product ID, not setting it manually
                                ProductName = productVM.Products.Name
                            };
                            _dbContext.PImages.Add(addressImage);
                        }
                        else
                        {
                            if (ProductToEdit.HomeImgUrl == "")
                            {
                                string homeImgUrl = item.FileName;
                                if (homeImgUrl.Contains("Home"))
                                {
                                    homeImgUrl = UploadFiles(item);
                                    ProductToEdit.HomeImgUrl = homeImgUrl;
                                }
                            }
                        }
                    }
                }

                // Ensure the ProductToEdit is updated and not trying to insert a new record.
                _dbContext.Products.Update(ProductToEdit);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index", "Product");
        }
        [HttpGet]
        public IActionResult Delete(int Id)
        {
            var ProductToDelete = _dbContext.Products.FirstOrDefault(u => u.Id == Id);
            var ImagesTODelete = _dbContext.PImages.Where(u => u.ProductId == Id).Select(u => u.ImgUrl);
            foreach (var image in ImagesTODelete)
            {
                string imageUrl = "Images\\" + image;
                var toDeleteImageFromFolder = Path.Combine(_HostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
                DeleteAImage(toDeleteImageFromFolder);
            }

            if (ProductToDelete.HomeImgUrl != "")
            {
                string imageUrl = "Images\\" + ProductToDelete.HomeImgUrl;
                var toDeleteImageFromFolder = Path.Combine(_HostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
                DeleteAImage(toDeleteImageFromFolder);
            }
            _dbContext.Products.Remove(ProductToDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index","Product");
        }

        private void DeleteAImage(string toDeleteImageFromFolder)
        {
            if (System.IO.File.Exists(toDeleteImageFromFolder)) {
                System.IO.File.Delete(toDeleteImageFromFolder);
            }
        }
    }
}
