using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public class ShippingZone
    {
        public int Id { get; set; }
        [Display(Name = "اسم المنطقة")]
        public string Name { get; set; }
        [Display(Name = "تكلفة الشحن")]
        public decimal Price { get; set; }
    }
}