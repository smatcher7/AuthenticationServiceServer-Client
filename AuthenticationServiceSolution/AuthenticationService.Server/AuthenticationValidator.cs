using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Server
{
    public class AuthenticationValidator : ISecurityTokenValidator
    {
        public bool CanReadToken(string securityToken) => true;

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "your string",
                ValidAudience = "your string",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your secrete code"))
            };

            var claimsPrincipal = handler.ValidateToken(securityToken, tokenValidationParameters, out validatedToken);
            return claimsPrincipal;
        }

        public bool CanValidateToken { get; } = true;
        public int MaximumTokenSizeInBytes { get; set; } = int.MaxValue;
    }
}