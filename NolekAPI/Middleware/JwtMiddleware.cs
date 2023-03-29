using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NolekAPI.Data;

namespace NolekAPI.Middleware
{
    [Authorize(Policy = "MyPolicy")]
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _policyName;
        private readonly string _jwtSecretKey;
        private readonly int _jwtExpirationMinutes;


        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, string policyName)
        {
            _next = next;
            _policyName = policyName;
            _jwtSecretKey = configuration.GetValue<string>("Jwt:Key");
            _jwtExpirationMinutes = configuration.GetValue<int>("Jwt:ExpirationInMinutes");
        }

        public async Task InvokeAsync(HttpContext context, IAuthorizationService authorizationService)
        {
            bool success = false;
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                success = AttachUserToContext(context, token, authorizationService);
            }
            await _next(context);
        }

        private bool AttachUserToContext(HttpContext context, string token, IAuthorizationService authorizationService)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecretKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "sub").Value;

                var claims = new List<Claim>(){
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };



                // Add the role claims to the list
                var roleClaims = jwtToken.Claims.Where(x => x.Type == ClaimTypes.Role);
                claims.AddRange(roleClaims);

                var identity = new ClaimsIdentity(claims, "jwt");
                Thread.CurrentPrincipal = new ClaimsPrincipal(identity);
                context.User = new ClaimsPrincipal(identity);

                // set the user as the current principal in the HttpContext
                //context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "jwt"));




                AuthorizationResult authorizationResult = authorizationService.AuthorizeAsync(context.User, null, _policyName).Result;

                if (!authorizationResult.Succeeded)
                {
                    context.ChallengeAsync();
                    return false;
                }

                return true;

            }
            catch
            {
                // do nothing if token validation fails
                return false;
            }


        }

        private string GenerateJwtToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
