
using Core;
using Core.Enteties;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP.Authentication;

public class JwtHandler
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationSection _jwtSection;

    public JwtHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtSection = _configuration.GetSection("JWT");
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtSection["Key"]!);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private List<Claim> GetClaims(AppUser user, IList<string> roles, int? plantImageId = null, int? problemId = null)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.GivenName, user.Name!),
            new Claim(ClaimTypes.Email, user.Email!),
           //new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim("id", user.Id.ToString()),
        };


        if (plantImageId.HasValue)
        {
            claims.Add(new Claim("plantImageId", plantImageId.Value.ToString())); // 🔥 إضافة `plantImageId`
        }
        if (problemId.HasValue)
        {
            claims.Add(new Claim("problemId", problemId.Value.ToString())); 
        }

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return claims;
    }

    private JwtSecurityToken GenerateSecurityToken(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var securityToken = new JwtSecurityToken(
            issuer: _jwtSection["ValidIssuer"],
            audience: _jwtSection["ValidAudience"],
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.Now.AddDays(Convert.ToDouble(_jwtSection["DurationInDays"]))
            );

        return securityToken;

    }

    public string CreateToken(AppUser user, IList<string> roles, int? plantImageId = null,int? problemId= null)
    {
        var claims = GetClaims(user, roles, plantImageId, problemId);
        var signingCredientials = GetSigningCredentials();
        var securityToken = GenerateSecurityToken(signingCredientials, claims);
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        Console.WriteLine($"Generated Token: {token}");

        return token;
    }

}
