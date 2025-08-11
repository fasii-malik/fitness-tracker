using FitnessTracker.Models.Entities;

namespace FitnessTracker.Models
{
    public class ShopVM
    {
        public IEnumerable<Product> ProductList { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public string searchByName { get; set; }

    }
}
