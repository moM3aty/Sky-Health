using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> FeaturedProducts { get; set; }

        public Dictionary<Category, List<Product>> ProductsByCategory { get; set; }

        public IEnumerable<Blog> LatestBlogs { get; set; }
    }
}
