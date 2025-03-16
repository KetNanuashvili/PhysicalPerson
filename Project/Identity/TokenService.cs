using Microsoft.IdentityModel.Tokens;
using Project.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var secretKey = "DDDDDJJJJJJJJFFFLLLLLLLLLLLLLDDDDDLLLLLLLLDDDD";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
    };

        if (user.Email == "ketinanuashvili@gmail.com")
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var token = new JwtSecurityToken(
            issuer: "TestIssuer",  
            audience: "TestAudience",  
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
