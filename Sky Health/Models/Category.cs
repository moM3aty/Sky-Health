using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sky_Health.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "اسم القسم")]
        [Required(ErrorMessage = "اسم القسم مطلوب")]
        public string Name { get; set; }
        [Display(Name = "المنتجات")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
