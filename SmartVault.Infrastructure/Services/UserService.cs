using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SmartVault.Core.BusinessObjects;
using SmartVault.Core.Settings;
using SmartVault.Infrastructure.Dto.UserService;
using SmartVault.Infrastructure.Interfaces;
using SmartVault.Shared.Interfaces;
using SmartVault.Shared.Utils;

namespace SmartVault.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private IDbContextService dbContextService;
        private ISettings settings;
        private readonly IPasswordHelper passwordHelper;
        
        public UserService(IDbContextService dbContextService, ISettings settings, IPasswordHelper passwordHelper)
        {
            this.dbContextService = dbContextService;
            this.settings = settings;
            this.passwordHelper = passwordHelper;
        }
        
        public async Task<AuthenticateResDto> AuthenticateAsync(AuthenticateReqDto dto)
        {
            var user =  dbContextService.GetAll<User>("[Username] = @Username",
                new {
                    dto.Username
                }).FirstOrDefault();

            // Check if the username exists.
            if (user == null) throw new Exception("Username is incorrect.");
            
            // Check if password is correct.
            if (!passwordHelper.VerifyHash(dto.Password, user.PasswordHash))
            {
                throw new Exception("Password is incorrect.");
            }

            // Authentication successful.
            return new AuthenticateResDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = CreateToken(user.Id.ToString())
            };
        }

        public async Task<GetDetailsResDto> RegisterAsync(RegisterReqDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Password is required.");

            var existingUser =  dbContextService.GetAll<User>("[Username] = @Username OR [Email] = @Email",
            new {
                dto.Username,
                dto.Email
            }).FirstOrDefault();
            
            if (existingUser?.Username == dto.Username)
                throw new Exception((string.Format("Username '{0}' is already taken.", dto.Username)));
            
            if (existingUser?.Email == dto.Email)
                throw new Exception(string.Format("Email '{0}' is already taken.", dto.Email));
            
            var passwordHash = passwordHelper.CreateHash(dto.Password);

            var user = new User
            {
                FirstName = dto.GivenName,
                LastName = dto.FamilyName,
                Username = dto.Username,
                // Email must be confirmed first.
            };
            var emailSuccess = await ChangeEmailAsync(user, dto.Email);
            user.CreatedAt = DateTime.UtcNow;
            user.Password = passwordHash.ToString();

            if (!emailSuccess) throw new Exception("Sending of confirmation email failed.");

            dbContextService.Insert(user);

            return new GetDetailsResDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                Email = user.Email,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
            };
        }
        
    private async Task<bool> ChangeEmailAsync(User user, string newEmail)
    {
        //Send email here
        return true;
    }
            
    private string CreateToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, userId)}),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    }
}