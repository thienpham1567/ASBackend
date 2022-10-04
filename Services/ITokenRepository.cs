using Backend.Models;

namespace Backend.Services
{
    public interface ITokenRepository
    {
        Task<TokenModel> GenerateToken(User user);
        string GenerateRefreshToken();

        Task<ApiResponse> ValidateToken(TokenModel tokenModel);
    }
}