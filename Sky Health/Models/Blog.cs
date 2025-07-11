using System;
using System.ComponentModel.DataAnnotations;

namespace Sky_Health.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Display(Name = "عنوان المقال")]
        [Required(ErrorMessage = "عنوان المقال مطلوب")]
        public string Title { get; set; }
        [Display(Name = "محتوى المقال")]
        [Required(ErrorMessage = "محتوى المقال مطلوب")]
        public string Content { get; set; }
        [Display(Name = "الكاتب")]
        public string Author { get; set; } = "سكاي ميديكال";
        [Display(Name = "صورة المقال")]
        public string? ImageUrl { get; set; }
        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
