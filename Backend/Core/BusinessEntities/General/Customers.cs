using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using Zambon.OrderManagement.Core.BusinessEntities.Base;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;

namespace Zambon.OrderManagement.Core.BusinessEntities.General
{
    [Table(nameof(Customers), Schema = nameof(General))]
    public class Customers : Entity
    {
        [Column(TypeName = "VARCHAR(500)")]
        public string? Name { get; set; }


        public virtual ICollection<Orders>? Orders { get; set; }
    }

    public class CustomersConfiguration : EntityConfiguration<Customers>
    {
        public override void Configure(EntityTypeBuilder<Customers> builder)
        {
            base.Configure(builder);
        }
    }
}