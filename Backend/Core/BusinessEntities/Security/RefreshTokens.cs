using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zambon.OrderManagement.Core.BusinessEntities.Base;
using Zambon.OrderManagement.Core.Helpers.ValueGenerators;

namespace Zambon.OrderManagement.Core.BusinessEntities.Security
{
    [Table(nameof(RefreshTokens), Schema = nameof(Security))]
    public class RefreshTokens : BaseEntity
    {
        [Key]
        public long ID { get; set; }


        [Column(TypeName = "DATETIME")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime Expiration { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? RevokedOn { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string? Token { get; set; }

        public long UserID { get; set; }
        public virtual Users? User { get; set; }


        public bool IsActive => RevokedOn == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expiration;
    }

    public class RefreshTokensConfiguration : BaseEntityConfiguration<RefreshTokens>
    {
        public override void Configure(EntityTypeBuilder<RefreshTokens> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.CreatedOn)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<DateTimeGenerator>();
        }
    }
}