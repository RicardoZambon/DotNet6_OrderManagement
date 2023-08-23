using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace Zambon.OrderManagement.Core.Helpers.ExtensionMethods
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<TEntity> ConfigureRelationEntity<TEntity, TRelatedEntity>(
            this EntityTypeBuilder<TEntity> entityBuilder,
            string tableSchema,
            Expression<Func<TEntity, IEnumerable<TRelatedEntity>?>>? destNavigationExpression,
            Expression<Func<TRelatedEntity, IEnumerable<TEntity>?>> sourceNavigationExpression,
            string sourceProperty,
            string targetProperty,
            params object[] hasData
        )
            where TEntity : class
            where TRelatedEntity : class
        {
            if (string.IsNullOrEmpty(sourceProperty))
            {
                sourceProperty = typeof(TRelatedEntity).Name;
            }

            if (string.IsNullOrEmpty(targetProperty))
            {
                targetProperty = typeof(TEntity).Name;
            }

            entityBuilder.HasMany(destNavigationExpression)
                .WithMany(sourceNavigationExpression)
                .UsingEntity<Dictionary<string, object>>(
                    t => t.HasOne<TRelatedEntity>()
                        .WithMany()
                        .HasForeignKey($"{targetProperty}ID")
                        .OnDelete(DeleteBehavior.NoAction),

                    s => s.HasOne<TEntity>()
                        .WithMany()
                        .HasForeignKey($"{sourceProperty}ID")
                        .OnDelete(DeleteBehavior.NoAction),

                    c =>
                    {
                        c.ToTable($"{typeof(TEntity).Name}{typeof(TRelatedEntity).Name}", tableSchema);

                        c.HasKey($"{targetProperty}ID", $"{sourceProperty}ID");

                        if (hasData?.Any() ?? false)
                        {
                            c.HasData(hasData);
                        }
                    }
                );

            return entityBuilder;
        }
    }
}