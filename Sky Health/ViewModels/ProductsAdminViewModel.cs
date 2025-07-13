using Microsoft.AspNetCore.Mvc.Rendering;
using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class ProductsAdminViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public SelectList Categories { get; set; }
        public string SearchTerm { get; set; }
        public int? CurrentCategoryId { get; set; }
        public string CurrentStockStatus { get; set; }
    }
}
