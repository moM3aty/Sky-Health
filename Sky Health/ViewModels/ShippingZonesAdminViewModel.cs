using Sky_Health.Models;
using System.Collections.Generic;

namespace Sky_Health.ViewModels
{
    public class ShippingZonesAdminViewModel
    {
        public IEnumerable<ShippingZone> ShippingZones { get; set; }
        public string SearchTerm { get; set; }
    }
}
