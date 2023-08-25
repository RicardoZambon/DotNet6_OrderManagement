using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Zambon.OrderManagement.Core.BusinessEntities.Base;

namespace Zambon.OrderManagement.Core.DataInitializers
{
    public abstract class BaseInitializer
    {
        protected const long DefaultUserID = 1;
        protected readonly AppDbContext dbContext;

        public BaseInitializer(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public abstract void Initialize();


        protected IDbContextTransaction BeginTransaction(string methodName)
        {
            return dbContext.Database.BeginTransaction();
        }

        protected void SaveContext<TEntity, TValue>(IEnumerable<TEntity> entities, string methodName, Func<TEntity, TValue> propertyCheck) where TEntity : Entity
        {
            var maxID = 1L;
            if (dbContext.Set<TEntity>().Any())
            {
                maxID = dbContext.Set<TEntity>().IgnoreQueryFilters().Max(x => x.ID) + 1;
            }

            if (entities.Max(x => x.ID) is long entitiesMaxID && entitiesMaxID > maxID)
            {
                maxID = entitiesMaxID + 1;
            }

            foreach (var entity in entities)
            {
                var propertyValue = new TEntity[] { entity }.Select(propertyCheck).FirstOrDefault();

                if (!dbContext.Set<TEntity>().Select(propertyCheck).Any(x => EqualityComparer<TValue>.Default.Equals(x, propertyValue)) && entity.ID <= 0)
                {
                    entity.ID = maxID;
                    maxID++;
                }
                else if (dbContext.Set<TEntity>().Select(propertyCheck).Any(x => EqualityComparer<TValue>.Default.Equals(x, propertyValue)) && entity.ID > 0)
                {
                    entity.ID = 0;
                }
            }

            SaveContext(entities.Where(x => x.ID > 0), methodName, EntityState.Added);
        }

        protected void SaveContext<TEntity>(IEnumerable<TEntity> entities, string methodName, EntityState entriesState = EntityState.Added) where TEntity : class
        {
            foreach (var entity in entities)
            {
                using var transaction = BeginTransaction(methodName);

                switch (entriesState)
                {
                    case EntityState.Added:
                        dbContext.Set<TEntity>().Add(entity);
                        break;
                    case EntityState.Modified:
                        dbContext.Set<TEntity>().Update(entity);
                        break;
                    case EntityState.Deleted:
                        dbContext.Set<TEntity>().Remove(entity);
                        break;
                }

                if (entriesState == EntityState.Added)
                {
                    SetIdentityInsert<TEntity>(true);
                }

                dbContext.SaveChanges();

                if (entriesState == EntityState.Added)
                {
                    SetIdentityInsert<TEntity>(false);
                }

                dbContext.SaveChanges();
                transaction.Commit();
            }
            dbContext.ChangeTracker.Clear();
        }

        protected void SetIdentityInsert<TEntity>(bool enable) where TEntity : class
        {
            if (dbContext.Model.FindEntityType(typeof(TEntity)) is IEntityType entityType)
            {
                dbContext.Database.ExecuteSqlRaw(
                    $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {(enable ? "ON" : "OFF")}"
                );
            }
        }
    }
}