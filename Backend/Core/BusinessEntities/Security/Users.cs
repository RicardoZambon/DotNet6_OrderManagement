using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using Zambon.OrderManagement.Core.BusinessEntities.Base;

namespace Zambon.OrderManagement.Core.BusinessEntities.Security
{
    [Table(nameof(Users), Schema = nameof(Security))]
    public class Users : Entity
    {
        [Column(TypeName = "VARCHAR(200)")]
        public string? Email { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string? Name { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string? Password { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string? Username { get; set; }


        public virtual ICollection<RefreshTokens>? RefreshTokens { get; set; }
    }

    public class UsersConfiguration : EntityConfiguration<Users>
    {
        public override void Configure(EntityTypeBuilder<Users> builder)
        {
            base.Configure(builder);
        }
    }
}