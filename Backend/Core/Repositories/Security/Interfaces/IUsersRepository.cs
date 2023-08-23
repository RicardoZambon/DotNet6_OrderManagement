using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Interfaces;

namespace Zambon.OrderManagement.Core.Repositories.Security.Interfaces
{
    public interface IUsersRepository
    {
        Task AddAsync(Users user);
        Task<Users?> FindByIdAsync(long userId);
        IQueryable<Users> FindByIds(IEnumerable<long> userIds);
        Task<Users?> FindByUsernameAsync(string username);
        IQueryable<Users> List(IListParameters parameters, ICollection<Users>? sourceCollection = null);
        Task RemoveAsync(long userId);
        Task UpdateAsync(Users user);
        Task ValidateAsync(Users user);
    }
}