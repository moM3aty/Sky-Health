using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalSales { get; set; }
        public int NewOrdersCount { get; set; }
        public int ProductsCount { get; set; }
        public int CategoriesCount { get; set; }
        public List<Order> LatestOrders { get; set; }
        public List<ActivityLog> RecentActivities { get; set; }

        public List<string> SalesChartLabels { get; set; }
        public List<decimal> SalesChartData { get; set; }
        public List<string> TopCategoriesChartLabels { get; set; }
        public List<int> TopCategoriesChartData { get; set; }
    }

    public class ActivityLog
    {
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string IconClass { get; set; }
        public string IconBgClass { get; set; }
    }
}
