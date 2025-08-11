using FitnessTracker.Models;
using FitnessTracker.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessTracker.Infrastructure
{
    //public sealed class TokenProvider
    //{
    //    private readonly IConfiguration _configuration;

    //    // Constructor
    //    public TokenProvider(IConfiguration configuration)
    //    {
    //        _configuration = configuration;
    //    }

    //    public string Create(Signup user)
    //    {
    //        // Get the secret key from configuration
    //        string secretKey = _configuration["Jwt:Secret"];
    //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

    //        // Create signing credentials
    //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    //        // Define claims
    //        var claims = new[]
    //        {
    //            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    //            new Claim(JwtRegisteredClaimNames.Email, user.Email)
    //        };

    //        // Create token descriptor
    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(claims),
    //            Expires = DateTime.UtcNow.AddMinutes(60),
    //            SigningCredentials = credentials,
    //            Issuer = _configuration["Jwt:Issuer"],
    //            Audience = _configuration["Jwt:Audience"]
    //        };

    //        // Create token handler and generate the token
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var token = tokenHandler.CreateToken(tokenDescriptor);

    //        // Return the serialized token
    //        return tokenHandler.WriteToken(token);
    //    }
    //}
}
