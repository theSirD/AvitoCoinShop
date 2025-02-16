using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AvitoCoinShop.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AvitoCoinShop.Application.Auth;


public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    
    private readonly string _jwtSecretKey;
    
    private readonly int _jwtExpirationDays;
    
    private readonly IOptions<JwtOptions> _jwtOptions;

    public AuthService(IUserService userService, IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions;
        _userService = userService;
        _jwtSecretKey = _jwtOptions.Value.Secret;
        _jwtExpirationDays = _jwtOptions.Value.TokenLifetimeDays;
    }

    public async Task<string> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
    {
        Models.Domain.Users.User? existingUser = await _userService.GetByUsernameAsync(username, cancellationToken);
        if (existingUser is null)
        {
            Models.Domain.Users.User newUser = new Models.Domain.Users.User(
                null,
                username,
                1000,
                BCrypt.Net.BCrypt.HashPassword(password),
                DateTime.UtcNow);
            long userId = await _userService.CreateUserAsync(newUser, cancellationToken);
           
            var newUserClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
            };

            var newUserKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var newUserCreds = new SigningCredentials(newUserKey, SecurityAlgorithms.HmacSha256);
            var newUserToken = new JwtSecurityToken(
                issuer: "AvitoCoinShop",
                audience: "AvitoCoinShopClient",
                claims: newUserClaims,
                expires: DateTime.UtcNow.AddDays(_jwtExpirationDays),
                signingCredentials: newUserCreds
            );

            return new JwtSecurityTokenHandler().WriteToken(newUserToken);
        }

        // TODO. Add custom exception
        if (!BCrypt.Net.BCrypt.Verify(password, existingUser.PasswordHash))
            throw new Exception("Wrong password");
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.UserId.ToString()),
            new Claim(ClaimTypes.Name, existingUser.Name),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "AvitoCoinShop",
            audience: "AvitoCoinShopClient",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_jwtExpirationDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
