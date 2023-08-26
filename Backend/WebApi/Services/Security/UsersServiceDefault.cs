using AutoMapper;
using AutoMapper.QueryableExtensions;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.General;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.WebApi.Models.General;
using Zambon.OrderManagement.WebApi.Models.Security;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Security
{
    /// <inheritdoc/>
    public class UsersServiceDefault : IUsersService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="UsersServiceDefault"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AppDbContext"/> instance.</param>
        /// <param name="mapper">The <see cref="IMapper"/> instance.</param>
        /// <param name="usersRepository">The <see cref="IUsersRepository"/> instance.</param>
        public UsersServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            IUsersRepository usersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.usersRepository = usersRepository;
        }


        /// <inheritdoc/>
        public async Task<UserUpdateModel> FindUserByIdAsync(long userId)
        {
            if (await usersRepository.FindByIdAsync(userId) is not Users user)
            {
                throw new EntityNotFoundException(nameof(Users), userId);
            }
            return mapper.Map<UserUpdateModel>(user);
        }

        /// <inheritdoc/>
        public async Task<UserUpdateModel> InsertNewUserAsync(UserInsertModel userModel)
        {
            var user = mapper.Map<Users>(userModel);

            await usersRepository.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return mapper.Map<UserUpdateModel>(user);
        }

        /// <inheritdoc/>
        public IEnumerable<UsersListModel> ListUsers(IListParameters parameters)
        {
            return usersRepository.List(parameters).ProjectTo<UsersListModel>(mapper.ConfigurationProvider);
        }

        /// <inheritdoc/>
        public async Task RemoveUsersAsync(long[] userIds)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var user in userIds)
                {
                    await usersRepository.RemoveAsync(user);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<UserUpdateModel> UpdateExistingUserAsync(UserUpdateModel userModel)
        {
            if (await usersRepository.FindByIdAsync(userModel.ID) is not Users user)
            {
                throw new EntityNotFoundException(nameof(Users), userModel.ID);
            }

            await usersRepository.UpdateAsync(mapper.Map(userModel, user));
            await dbContext.SaveChangesAsync();

            return mapper.Map<UserUpdateModel>(user);
        }
    }
}