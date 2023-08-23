using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using Zambon.OrderManagement.Core.BusinessEntities.Base;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Helpers.ValueGenerators;

namespace Zambon.OrderManagement.Core.BusinessEntities.Stock
{
    [Table(nameof(Orders), Schema = nameof(Stock))]
    public class Orders : Entity
    {
        [Column(TypeName = "DATETIME")]
        public DateTime CreatedOn { get; set; }

        public long CustomerID { get; set; }
        public virtual Customers? Customer { get; set; }


        public virtual ICollection<OrdersProducts>? Products { get; set; }
    }

    public class OrdersConfiguration : EntityConfiguration<Orders>
    {
        public override void Configure(EntityTypeBuilder<Orders> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.CreatedOn)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<DateTimeGenerator>()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}