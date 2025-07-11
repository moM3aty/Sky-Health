using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public class PharmacyAccessCode
    {
        public int Id { get; set; }
        [Display(Name = "كود الدخول")]
        public string Code { get; set; }
        [Display(Name = "نشط؟")]
        public bool IsActive { get; set; } = true;
    }
}