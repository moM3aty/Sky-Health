using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Display(Name = "اسم العميل")]
        public string CustomerName { get; set; }
        [Display(Name = "العنوان")]
        public string Address { get; set; }
        [Display(Name = "منطقة الشحن")]
        public string ShippingZoneName { get; set; }
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }
        [Display(Name = "إجمالي المبلغ")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "تكلفة الشحن")]
        public decimal ShippingCost { get; set; }
        [Display(Name = "طريقة الدفع")]
        public string PaymentMethod { get; set; }
        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Display(Name = "حالة الطلب")]
        public string OrderStatus { get; set; }
        [Display(Name = "عناصر الطلب")]
        public bool IsNew { get; set; } = true;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}