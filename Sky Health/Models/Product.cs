using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public enum ProductType
    {
        [Display(Name = "مستخدم عادي")]
        Regular,
        [Display(Name = "صيدلية")]
        Pharmacy,

        [Display(Name = "معمل")] 
        Lab
    }

    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(100, ErrorMessage = "اسم المنتج لا يمكن أن يتجاوز 100 حرف")]
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; }
        [Required(ErrorMessage = "الوصف مطلوب")]
        [StringLength(500, ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف")]
        [Display(Name = "الوصف")]
        public string Description { get; set; }
        [Display(Name = "رابط الصورة")]
        public string? ImageUrl { get; set; }
        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, 1000000, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        [Display(Name = " السعر بعد الخصم")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [Range(0.01, 1000000, ErrorMessage = "السعر قبل الخصم يجب أن يكون أكبر من صفر")]
        [Display(Name = "السعر قبل الخصم")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal? OldPrice { get; set; }
        [Required(ErrorMessage = "نوع المنتج مطلوب")]
        [Display(Name = "نوع المنتج")]
        public ProductType Type { get; set; }
        [Display(Name = "منتج مميز")]
        public bool IsFeatured { get; set; } = false;
        [Required(ErrorMessage = "القسم مطلوبة")]
        [Display(Name = "القسم")]
        public int CategoryId { get; set; }
        [Display(Name = "عدد الوحدات داخل العلبة")]
        [Range(1, int.MaxValue, ErrorMessage = "عدد الوحدات يجب أن يكون 1 على الأقل.")]
        public int? UnitsPerBox { get; set; }
        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون رقم موجب.")]
        [Display(Name = "الكمية بالمخزن")]
        public int StockQuantity { get; set; }
        [Display(Name = "القسم")]
        public virtual Category Category { get; set; }
    }
}