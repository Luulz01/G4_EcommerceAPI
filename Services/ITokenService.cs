using EcommerceAPI.Models;

namespace EcommerceAPI.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}