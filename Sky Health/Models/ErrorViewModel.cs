using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public class ErrorViewModel
    {
        [Display(Name = "رقم الطلب")]
        public string? RequestId { get; set; }
        [Display(Name = "إظهار رقم الطلب")]
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}