using AutoMapper;
using AutoMapper.QueryableExtensions;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Security;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Security
{
    public class UsersServiceDefault : IUsersService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;

        public UsersServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            IUsersRepository usersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.usersRepository = usersRepository;
        }


        public async Task<UserUpdateModel> FindUserByIdAsync(long userId)
        {
            return mapper.Map<UserUpdateModel>(await usersRepository.FindByIdAsync(userId));
        }

        public async Task<UserUpdateModel> InsertNewUserAsync(UserInsertModel userModel)
        {
            var user = mapper.Map<Users>(userModel);

            await usersRepository.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return mapper.Map<UserUpdateModel>(user);
        }

        public IEnumerable<UsersListModel> ListUsers(IListParameters parameters)
        {
            return usersRepository.List(parameters).ProjectTo<UsersListModel>(mapper.ConfigurationProvider);
        }

        public async Task RemoveCustomersAsync(long[] userIds)
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

        public async Task<UserUpdateModel> UpdateExistingCustomerAsync(UserUpdateModel userModel)
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