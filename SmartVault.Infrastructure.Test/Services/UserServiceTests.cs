using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SmartVault.Core.BusinessObjects;
using SmartVault.Core.Settings;
using SmartVault.Infrastructure.Dto.UserService;
using SmartVault.Infrastructure.Services;
using SmartVault.Shared.Interfaces;
using SmartVault.Shared.Utils;
using Xunit;

namespace SmartVault.Infrastructure.Test.Services
{
    public class UserServiceTests
    {
        private readonly UserService service;
        private readonly Mock<ISettings> mockSettings;
        private readonly Mock<IDbContextService> mockDbContextService;
        private readonly Mock<IPasswordHelper> passwordHelper;

        public UserServiceTests()
        {
            mockSettings = new Mock<ISettings>();
            mockSettings.Setup(x => x.Secret).Returns("random secret here,beautiful red flower");
            mockDbContextService = new Mock<IDbContextService>();
            passwordHelper = new Mock<IPasswordHelper>();
            service = new UserService(mockDbContextService.Object, mockSettings.Object, passwordHelper.Object);
        }
        [Fact]
        public async Task AuthenticateAsync_WhenCalled_AuthenticatesUser()
        {
            const string password = "AbcAbc123";
            User user = new User()
            {
                Id = 2,
                Username = "joedoe123",
                Password = password,
                PasswordHash = Encoding.UTF8.GetBytes("<<randomHashHere>>")
            };

            passwordHelper.Setup(x => x.VerifyHash(user.Password, user.PasswordHash))
                .Returns(true);
            mockDbContextService.Setup(x => x.GetAll<User>(
                    It.Is<string>(y => y == "[Username] = @Username")
                ,It.IsAny<object>(),null)
            ).Returns(new List<User>() {user});
            var dto = new AuthenticateReqDto
            {
                Username = user.Username,
                Password = password
            };

            var response = await service.AuthenticateAsync(dto);

            Assert.Equal(dto.Username, response.Username);
            Assert.Equal(user.FirstName, response.FirstName);
            Assert.Equal(user.LastName, response.LastName);
            Assert.Equal(user.Email, response.Email);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mockSettings.Object.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            tokenHandler.ValidateToken(response.Token, validationParameters, out _);
        }
    }
}