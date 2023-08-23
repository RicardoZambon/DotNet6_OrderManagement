using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using System.Net;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Services.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Security
{
    public class CurrentUserProviderDefault : ICurrentUserProvider
    {
        private const long DEFAULT_USER_ID = 1;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUsersRepository usersRepository;

        public CurrentUserProviderDefault(
            IHttpContextAccessor httpContextAccessor,
            IUsersRepository usersRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.usersRepository = usersRepository;
        }


        public IPAddress? GetClientIPAddress()
        {
            return httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress;
        }

        public long GetInternalServiceUserID()
        {
            if (GetUserID() is long userID && userID == 0)
            {
                userID = DEFAULT_USER_ID;
            }
            return userID;
        }

        public async Task<Users?> GetUserAsync()
        {
            return await usersRepository.FindByIdAsync(GetInternalServiceUserID());
        }

        public long GetUserID()
        {
            return Convert.ToInt64(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(a => a.Type == "uid")?.Value);
        }
    }
}