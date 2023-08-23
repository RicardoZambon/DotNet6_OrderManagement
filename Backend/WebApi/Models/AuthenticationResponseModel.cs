using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.WebApi.Models
{
    [AutoMap(typeof(Users), ReverseMap = false)]
    public class AuthenticationResponseModel
    {
        public string? Email { get; set; }

        [Ignore]
        public string? RefreshToken { get; set; }

        [Ignore]
        public DateTime RefreshTokenExpiration { get; set; }

        [Ignore]
        public string? Token { get; set; }

        public string? Username { get; set; }
    }
}