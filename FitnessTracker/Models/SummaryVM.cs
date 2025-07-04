using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessTracker.Models
{
    public class SummaryVM
    {
        public IEnumerable<UserCart> userCartList { get; set; }
        public UserOrderHeader? orderSummary { get; set; }

        public string? cartUserId { get; set; }

        public IEnumerable<SelectListItem> paymentOptions { get; set; }

        public double? paymentPaidByCard { get; set; } = 0.0;
    }
}
