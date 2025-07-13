using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class OrdersAdminViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public string SearchTerm { get; set; }
        public string CurrentStatus { get; set; }
    }
}
