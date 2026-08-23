using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Veesh.Core.DTOs.Auth;
using Veesh.Core.DTOs.User;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;

namespace Veesh.Core.Services;

public class AuthService(
    IUserRepository userRepository,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration)
{
    public async Task<UserResponse> Login(LoginRequest request)
    {
        var user = await userRepository.GetUserByUsernameAsync(request.Username);
        if (user == null) throw new NotFoundException("User not found.");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid username or password.");

        var roles = await userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        var jwtSettings = configuration.GetSection("JwtSettings");
        var expirationDays = jwtSettings.GetSection("ExpirationDays").Get<int>();

        var context = httpContextAccessor.HttpContext!;
        context.Response.Cookies.Append("Veesh.Token", token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddDays(expirationDays)
        });

        return user.Adapt<UserResponse>();
    }

    public async Task<UserResponse> Register(RegisterRequest request)
    {
        var newUser = new ApplicationUser
        {
            PhoneNumber = request.PhoneNumber,
            UserName = request.UserName,
            Email = request.Email,
            Name = request.Name,
        };
        var result = await userRepository.CreateAsync(newUser, request.Password);
        return result.Adapt<UserResponse>();
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.PhoneNumber!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(jwtSettings.GetSection("ExpirationDays").Get<int>()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}