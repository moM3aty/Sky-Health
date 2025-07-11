using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Sky_Health.ViewModels
{
    public class CheckoutViewModel
    {

        [BindNever]
        public CartViewModel Cart { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار منطقة الشحن")]
        public int ShippingZoneId { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار طريقة الدفع")]
        public string PaymentMethod { get; set; }
    }
}