using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;

namespace Zambon.OrderManagement.Core.Repositories.Security
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext dbContext;

        public UsersRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task AddAsync(Users user)
        {
            await ValidateAsync(user);

            await dbContext.Set<Users>().AddAsync(user);
        }

        public async Task<Users?> FindByIdAsync(long userId)
            => await dbContext.FindAsync<Users>(userId);

        public IQueryable<Users> FindByIds(IEnumerable<long> userIds)
            => dbContext.Set<Users>().Where(x => userIds.Contains(x.ID));

        public async Task<Users?> FindByUsernameAsync(string username)
            => await dbContext.Set<Users>()
                .FirstOrDefaultAsync(u => EF.Functions.Like(u.Username ?? string.Empty, username));

        public IQueryable<Users> List(IListParameters parameters, ICollection<Users>? sourceCollection = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var list = sourceCollection == null ? dbContext.Set<Users>().AsQueryable() : sourceCollection.AsQueryable();

            if (parameters?.Filters?.Any() ?? false)
            {
                var filters = new Dictionary<string, object>(parameters.Filters, StringComparer.InvariantCultureIgnoreCase);

                if (filters.ContainsKey(nameof(Users.Username)))
                {
                    list = list.Where(x => (x.Username ?? string.Empty).Contains(filters[nameof(Users.Username)].ToString() ?? string.Empty));
                }
            }

            list = list.OrderBy(x => x.Username);

            if (parameters?.StartRow > 0)
            {
                list = list.Skip(parameters.StartRow);
            }

            if (parameters?.EndRow > 0)
            {
                list = list.Take(parameters.EndRow);
            }

            return list;
        }

        public async Task RemoveAsync(long userId)
        {
            if (await FindByIdAsync(userId) is not Users user)
            {
                throw new EntityNotFoundException(nameof(Users), userId);
            }

            user.IsDeleted = true;

            dbContext.Set<Users>().Update(user);
        }

        public async Task UpdateAsync(Users user)
        {
            await ValidateAsync(user);

            dbContext.Set<Users>().Update(user);
        }

        public async Task ValidateAsync(Users user)
        {
            var result = new ValidationResult();

            // Username
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                result.SetError(nameof(Users.Username), "required");
            }
            else if (await dbContext.Set<Users>().AnyAsync(x => EF.Functions.Like(x.Username ?? string.Empty, user.Username) && x.ID != user.ID))
            {
                result.SetError(nameof(Users.Username), "exists");
            }

            if (result.Errors.Any())
            {
                throw new EntityValidationFailureException(nameof(Users), user.ID, result);
            }
        }
    }
}