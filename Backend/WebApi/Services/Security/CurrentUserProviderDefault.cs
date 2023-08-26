using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.Core.Services.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Security
{
    /// <summary>
    /// Service for providing the authenticated user in application.
    /// </summary>
    public class CurrentUserProviderDefault : ICurrentUserProvider
    {
        private const long DEFAULT_USER_ID = 1;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUsersRepository usersRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="CurrentUserProviderDefault"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> instance.</param>
        /// <param name="usersRepository">The <see cref="IUsersRepository"/> instance.</param>
        public CurrentUserProviderDefault(
            IHttpContextAccessor httpContextAccessor,
            IUsersRepository usersRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.usersRepository = usersRepository;
        }


        /// <summary>
        /// Return the logged user ID, or the ID of the internal user when running the application in service mode.
        /// </summary>
        /// <returns>The ID of the logged user, or the ID of the internal user for service mode.</returns>
        public long GetInternalServiceUserID()
        {
            if (GetUserID() is long userID && userID == 0)
            {
                userID = DEFAULT_USER_ID;
            }
            return userID;
        }

        /// <summary>
        /// Return the logged user instance.
        /// </summary>
        /// <returns>An instance of logged <see cref="Users"/>.</returns>
        public async Task<Users?> GetUserAsync()
        {
            return await usersRepository.FindByIdAsync(GetInternalServiceUserID());
        }

        /// <summary>
        /// Return the logged user ID.
        /// </summary>
        /// <returns>The ID of the logged user.</returns>
        public long GetUserID()
        {
            return Convert.ToInt64(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(a => a.Type == "uid")?.Value);
        }
    }
}