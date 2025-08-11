using FitnessTracker.Models.Entities;

namespace FitnessTracker.Models
{
    public class CartIndexVM
    {
        public IEnumerable<UserCart> productList { get; set; }
    }
}
