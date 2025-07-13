using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class PharmacyCodesAdminViewModel
    {
        public IEnumerable<PharmacyAccessCode> PharmacyAccessCodes { get; set; }
        public string SearchTerm { get; set; }
    }
}