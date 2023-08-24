using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Security;

namespace Zambon.OrderManagement.WebApi.Services.Security.Interfaces
{
    public interface IUsersService
    {
        Task<UserUpdateModel> FindUserByIdAsync(long userId);
        Task<UserUpdateModel> InsertNewUserAsync(UserInsertModel userModel);
        IEnumerable<UsersListModel> ListUsers(IListParameters parameters);
        Task RemoveUsersAsync(long[] userIds);
        Task<UserUpdateModel> UpdateExistingUserAsync(UserUpdateModel userModel);
    }
}