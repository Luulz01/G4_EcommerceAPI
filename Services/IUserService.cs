using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(RegisterUserDto dto);
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, RegisterUserDto dto);
        Task<bool> DeleteAsync(int id);
        Task<UserResponseDto?> ValidateCredentialsAsync(LoginDto dto);
        Task<string?> LoginAsync(LoginDto dto, ITokenService tokenService);
    }
}