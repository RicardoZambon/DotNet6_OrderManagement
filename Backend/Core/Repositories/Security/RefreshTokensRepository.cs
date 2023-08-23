using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;

namespace Zambon.OrderManagement.Core.Repositories.Security
{
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly AppDbContext dbContext;

        public RefreshTokensRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<RefreshTokens?> FindByUsernameAndTokenAsync(string username, string refreshToken)
        {
            return await dbContext.Set<RefreshTokens>()
                .FirstOrDefaultAsync(t => EF.Functions.Like(t.User != null ? t.User.Username ?? string.Empty : string.Empty, username) && EF.Functions.Like(t.Token ?? string.Empty, refreshToken));
        }

        public async Task InsertAsync(RefreshTokens token)
        {
            await dbContext.Set<RefreshTokens>().AddAsync(token);
        }

        public async Task RevokeAsync(RefreshTokens token)
        {
            token.RevokedOn = DateTime.UtcNow;
            await Task.Run(() => { dbContext.Set<RefreshTokens>().Update(token); });
        }
    }
}