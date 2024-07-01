namespace SmartVault.Infrastructure.Dto.UserService
{
    public class RegisterReqDto
    {
        public string Username { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}