using System;
using System.Threading.Tasks;
using SmartVault.Infrastructure.Dto.UserService;
using SmartVault.Shared.Pagination;

namespace SmartVault.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResDto> AuthenticateAsync(AuthenticateReqDto dto);
        
        Task<GetDetailsResDto> RegisterAsync(RegisterReqDto dto);
        
    }
}