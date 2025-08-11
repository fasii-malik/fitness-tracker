using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Models.Entities
{
    public class PImages
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public string ImgUrl { get; set; }

        public string ProductName { get; set; }
    }
}
