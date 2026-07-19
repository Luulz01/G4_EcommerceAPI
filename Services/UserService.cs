using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            var existing = await _repository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("El email ya está registrado");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(user);
            return ToDto(user);
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user == null ? null : ToDto(user);
        }

        public async Task<bool> UpdateAsync(int id, RegisterUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            user.Name = dto.Name;
            user.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            await _repository.DeleteAsync(user);
            return true;
        }

        public async Task<UserResponseDto?> ValidateCredentialsAsync(LoginDto dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            return valid ? ToDto(user) : null;
        }

        private static UserResponseDto ToDto(User u) => new()
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        };

        public async Task<string?> LoginAsync(LoginDto dto, ITokenService tokenService)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return null;

            return tokenService.GenerateToken(user);
        }
    }
}