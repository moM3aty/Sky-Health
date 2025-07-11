using Sky_Health.Models;
using System.Threading.Tasks;

namespace Sky_Health.Services
{
    public interface IPaymobService
    {
        Task<string> CreatePaymentUrl(Order order);
    }
}