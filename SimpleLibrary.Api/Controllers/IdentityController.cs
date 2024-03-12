using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleLibrary.Api.Identity;
using SimpleLibrary.Common.Abstraction;
using SimpleLibrary.Common.Dtos;

namespace SimpleLibrary.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController: ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

    public IdentityController(IConfiguration configuration, IUserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost("token")]
    public  async Task<IActionResult> GenerateToken([FromBody] TokenGenerationRequest request)
    {
        var user = await _userService.GetByEmail(request.Email);
        var jwt = GetJwt(user, request);
        return Ok(jwt);
    }

    private string GetJwt(UserDto user, TokenGenerationRequest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:Key")!);

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("userId", user.UserName)
        };

        foreach (var claimPair in request.CustomClaims)
        {
            var jsonElement = (JsonElement) claimPair.Value;
            var valueType = jsonElement.ValueKind switch
            {
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                JsonValueKind.Number => ClaimValueTypes.Double,
                _ => ClaimValueTypes.String
            };

            var claim = new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType);
            claims.Add(claim);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            Issuer = _configuration.GetValue<string>("JwtSettings:Issuer")!,
            Audience = _configuration.GetValue<string>("JwtSettings:Audience")!,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}