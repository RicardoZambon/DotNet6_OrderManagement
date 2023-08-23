using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using Zambon.OrderManagement.Core.BusinessEntities.Base;

namespace Zambon.OrderManagement.Core.BusinessEntities.Stock
{
    [Table(nameof(Products), Schema = nameof(Stock))]
    public class Products : Entity
    {
        [Column(TypeName = "VARCHAR(500)")]
        public string? Name { get; set; }

        public decimal UnitPrice { get; set; }
    }

    public class ProductsConfiguration : EntityConfiguration<Products>
    {
        public override void Configure(EntityTypeBuilder<Products> builder)
        {
            base.Configure(builder);
        }
    }
}