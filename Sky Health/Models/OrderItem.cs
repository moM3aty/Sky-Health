using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [Display(Name = "رقم الطلب")]
        public int OrderId { get; set; }
        [Display(Name = "رقم المنتج")]
        public int ProductId { get; set; }
        [Display(Name = "اسم المنتج")]
        public string ProductName { get; set; }
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }
        [Display(Name = "سعر الوحدة")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "الطلب")]
        public virtual Order Order { get; set; }
        [Display(Name = "المنتج")]
        public virtual Product Product { get; set; }
    }
}