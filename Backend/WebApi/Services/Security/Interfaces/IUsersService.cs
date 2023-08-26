using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models.Security;

namespace Zambon.OrderManagement.WebApi.Services.Security.Interfaces
{
    /// <summary>
    /// Service for viewing and updating the <see cref="Users"/>.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Return a user by the ID.
        /// </summary>
        /// <param name="userId">The ID of the user to search for.</param>
        /// <returns>An instance of <see cref="UserUpdateModel"/> representing the properties from the found <see cref="Users"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task<UserUpdateModel> FindUserByIdAsync(long userId);
        /// <summary>
        /// Validate and insert a new user.
        /// </summary>
        /// <param name="userModel">The <see cref="UserInsertModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="UserUpdateModel"/> representing the properties from the inserted <see cref="Users"/> instance.</returns>
        /// <exception cref="EntityValidationFailureException">If the <see cref="UserInsertModel"/> has validation errors.</exception>
        Task<UserUpdateModel> InsertNewUserAsync(UserInsertModel userModel);
        /// <summary>
        /// Return a list of users.
        /// </summary>
        /// <param name="parameters">The <see cref="IListParameters"/> instance for pagination and filtering the results.</param>
        /// <returns>A list of <see cref="UsersListModel"/> representing the properties from the <see cref="Users"/> list.</returns>
        /// <exception cref="ArgumentNullException">If the <see cref="IListParameters" /> is null.</exception>
        IEnumerable<UsersListModel> ListUsers(IListParameters parameters);
        /// <summary>
        /// Remove existing users.
        /// </summary>
        /// <param name="userIds">The list of user IDs to be deleted.</param>
        /// <returns>A task the represents the asynchronous delete operation.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task RemoveUsersAsync(long[] userIds);
        /// <summary>
        /// Validate and update an existing user.
        /// </summary>
        /// <param name="userModel">The <see cref="UserUpdateModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="UserUpdateModel"/> representing the properties from the updated <see cref="Users"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        /// <exception cref="EntityValidationFailureException">If the <see cref="UserUpdateModel"/> has validation errors.</exception>
        Task<UserUpdateModel> UpdateExistingUserAsync(UserUpdateModel userModel);
    }
}