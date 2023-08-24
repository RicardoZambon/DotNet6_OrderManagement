using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Security
{
    public class AuthenticationServiceDefault : IAuthenticationService
    {
        private readonly IConfiguration config;
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;
        private readonly IRefreshTokensRepository refreshTokenRepository;

        public AuthenticationServiceDefault(
            IConfiguration config,
            AppDbContext dbContext,
            IMapper mapper,
            IUsersRepository usersRepository,
            IRefreshTokensRepository refreshTokenRepository)
        {
            this.config = config;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.usersRepository = usersRepository;
            this.refreshTokenRepository = refreshTokenRepository;
        }


        public async Task<AuthenticationResponseModel> RefreshTokenAsync(RefreshTokenModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.RefreshToken))
            {
                throw new RefreshTokenNotFoundException();
            }

            var refreshToken = await refreshTokenRepository.FindByUsernameAndTokenAsync(model.Username, model.RefreshToken);
            if (refreshToken == null)
            {
                throw new RefreshTokenNotFoundException();
            }
            else if (!refreshToken.IsActive || refreshToken.User == null)
            {
                throw new InvalidRefreshTokenException();
            }

            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var authenticationModel = mapper.Map<AuthenticationResponseModel>(refreshToken.User);

                await refreshTokenRepository.RevokeAsync(refreshToken);

                authenticationModel.Token = CreateJwtToken(refreshToken.User);

                refreshToken = CreateRefreshToken(refreshToken.User);
                await refreshTokenRepository.InsertAsync(refreshToken);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                authenticationModel.RefreshToken = refreshToken.Token;
                authenticationModel.RefreshTokenExpiration = refreshToken.Expiration;

                return authenticationModel;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AuthenticationResponseModel> SignInAsync(SignInModel model)
        {
            if ((await usersRepository.FindByUsernameAsync(model.Username ?? string.Empty)) is not Users user
                || user.Username == null || string.IsNullOrEmpty(model.Password)) {
                throw new InvalidAuthenticationException();
            }

            if (string.IsNullOrEmpty(user.Password)
                || user.Password != model.Password) //TODO: !PasswordHasher.VerifyHashedPassword(user.Senha, model.Password)
            {
                throw new InvalidAuthenticationException();
            }
            
            var refreshToken = CreateRefreshToken(user);

            var authenticationModel = mapper.Map<AuthenticationResponseModel>(user);
            authenticationModel.Token = CreateJwtToken(user);
            authenticationModel.RefreshToken = refreshToken.Token;
            authenticationModel.RefreshTokenExpiration = refreshToken.Expiration;

            await refreshTokenRepository.InsertAsync(refreshToken);

            await dbContext.SaveChangesAsync();
            return authenticationModel;
        }


        private string CreateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"])),
                    SecurityAlgorithms.HmacSha256
                ),

                Subject = new ClaimsIdentity(
                    new Claim[] {
                        new Claim("uid", user.ID.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Username ?? string.Empty)
                    }
                ),
                Issuer = config["JWT:Issuer"],
                Audience = config["JWT:Audience"],

                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(config["JWT:DurationInMinutes"]))
            });

            return tokenHandler.WriteToken(token);
        }

        private RefreshTokens CreateRefreshToken(Users user)
        {
            var randomNumber = RandomNumberGenerator.GetBytes(32);

            return dbContext.CreateProxy<RefreshTokens>(x =>
            {
                x.UserID = user.ID;
                x.User = user;
                x.Token = Convert.ToBase64String(randomNumber);
                x.Expiration = DateTime.UtcNow.AddDays(Convert.ToInt32(config["JWT:RefreshTokenExpiration"]));
            });
        }
    }
}