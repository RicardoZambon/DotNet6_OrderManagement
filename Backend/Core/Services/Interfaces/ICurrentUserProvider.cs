using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.Core.Services.Interfaces
{
    public interface ICurrentUserProvider
    {
        Task<Users?> GetUserAsync();
        long GetInternalServiceUserID();
        long GetUserID();
    }
}