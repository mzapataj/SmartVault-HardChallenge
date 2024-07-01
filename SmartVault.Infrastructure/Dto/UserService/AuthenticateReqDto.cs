

namespace SmartVault.Infrastructure.Dto.UserService
{
    /// <summary>
    /// Data transfer object for the <see cref="IUserService.AuthenticateAsync"/> request.
    /// </summary>
    public class AuthenticateReqDto
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
    }
}