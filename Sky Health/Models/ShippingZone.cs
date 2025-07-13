using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public class ShippingZone
    {
        public int Id { get; set; }
        [Display(Name = "اسم المنطقة")]
        public string Name { get; set; }
        [Display(Name = "تكلفة الشحن")]
        [DisplayFormat(DataFormatString = "{0:G29}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
    }
}