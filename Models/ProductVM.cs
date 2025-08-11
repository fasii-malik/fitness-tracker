using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessTracker.Models
{
    public class ProductVM
    {
        public Product? Products { get; set; }
        public IEnumerable<SelectListItem> CategoriesList { get; set; }
        public IEnumerable<IFormFile> Images {  get; set; }
        public Inventory? Inventories { get; set; }
        public PImages? PImages { get; set; }
    }
}
