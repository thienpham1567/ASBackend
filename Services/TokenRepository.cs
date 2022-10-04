using Backend.Data;
using Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AppDbContext _context;
        private readonly Jwt _jwtSettings;

        public TokenRepository(AppDbContext context, IOptionsMonitor<Jwt> optionsMonitors)
        {
            _context = context;
            _jwtSettings = optionsMonitors.CurrentValue;
        }
        public async Task<TokenModel> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key!);
            var securityKeyBytes = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKeyBytes, SecurityAlgorithms.HmacSha512Signature); // hashing algorithm
            var claims = new[]
            {
                new Claim("Username",user.Username),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(ClaimTypes.Name,user.FullName),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim("Id",user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token to database;
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                Token = refreshToken,
                UserId = user.Id,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };
            await _context.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public async Task<ApiResponse> ValidateToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key!);
            var tokenValidationParameters = new TokenValidationParameters
            {
                // Tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,
                // Ký vào token
                ValidateIssuerSigningKey = true,
                // Sài thuật toán đối xứng tự động mã hóa
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                // Check 1: AccessTOken valid format
                var tokenVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidationParameters, out var validatedToken);

                // Check 2: Check is algorithm matched ?
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)//false
                    {
                        return new ApiResponse
                        {
                            Success = false,
                            Message = "Invalid token"
                        };
                    }
                }

                // Check 3: Check is accessToken expired?
                var utcExpireDate = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Access token hasn't expired"
                    };
                }

                // Check 4: Check refreshToken exists in Db ? 
                var existToken = _context.RefreshTokens.FirstOrDefault(t => t.Token == model.RefreshToken);
                if (existToken == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token doesn't exist"
                    };
                }

                //Check 5: Check refreshToken is used or revoked ?
                if (existToken.IsUsed)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token are being used"
                    };
                }

                if (existToken.IsRevoked)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token is revoked"
                    };
                }

                //Check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenVerification.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
                if (existToken.JwtId != jti)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    };
                }

                //Update token is used
                existToken.IsRevoked = true;
                existToken.IsUsed = true;

                _context.Update(existToken);
                await _context.SaveChangesAsync();

                var user = await _context.Users!.SingleOrDefaultAsync(u => u.Id == existToken.UserId);
                // Create new token
                var token = await GenerateToken(user);
                return new ApiResponse
                {
                    Success = true,
                    Message = "Renew Token successfully",
                    Data = token,
                };

            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Somethings went wrong",
                };
            }
            return new ApiResponse
            {
                Success = false,
                Message = "Somethings went wrong",
            };
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}