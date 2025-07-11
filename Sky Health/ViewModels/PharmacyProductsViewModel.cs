using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class PharmacyProductsViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> AllCategories { get; set; }
        public int? CurrentCategoryId { get; set; }
        public string SearchTerm { get; set; }
    }
}
