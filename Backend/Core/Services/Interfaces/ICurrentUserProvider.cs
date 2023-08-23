using System.Net;
using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.Core.Services.Interfaces
{
    public interface ICurrentUserProvider
    {
        Task<Users?> GetUserAsync();
        IPAddress? GetClientIPAddress();
        long GetInternalServiceUserID();
        long GetUserID();
    }
}