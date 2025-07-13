using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class BlogsAdminViewModel
    {
        public IEnumerable<Blog> Blogs { get; set; }
        public string SearchTerm { get; set; }
    }
}
