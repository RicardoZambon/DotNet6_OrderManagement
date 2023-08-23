using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using Zambon.OrderManagement.Core.BusinessEntities.Base;

namespace Zambon.OrderManagement.Core.BusinessEntities.Stock
{
    [Table(nameof(OrdersProducts), Schema = nameof(Stock))]
    public class OrdersProducts : Entity
    {
        public long OrderID { get; set; }
        public virtual Orders? Order { get; set; }

        public long ProductID { get; set; }
        public virtual Products? Product { get; set; }

        public int Qty { get; set; }

        public decimal UnitPrice { get; set; }
    }

    public class OrdersProductsConfiguration : EntityConfiguration<OrdersProducts>
    {
        public override void Configure(EntityTypeBuilder<OrdersProducts> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Order)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}