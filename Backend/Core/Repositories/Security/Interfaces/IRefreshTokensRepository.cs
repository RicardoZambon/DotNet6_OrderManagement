using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.Core.Repositories.Security.Interfaces
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshTokens?> FindByUsernameAndTokenAsync(string username, string refreshToken);

        Task InsertAsync(RefreshTokens token);
        Task RevokeAsync(RefreshTokens token);
    }
}