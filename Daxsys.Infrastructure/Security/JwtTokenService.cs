using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Daxsys.Application.Common.Interfaces;
using Daxsys.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Daxsys.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user, bool isAdmin, DateTime expiresAt)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"]!;
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.IdUsuario),
            new(JwtRegisteredClaimNames.UniqueName, user.IdUsuario),
            new(ClaimTypes.NameIdentifier, user.IdUsuario),
            new(ClaimTypes.Name, user.IdUsuario),
            new("is_admin", isAdmin.ToString().ToLower())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateContextToken(string userId,bool isAdmin,int EmpCodigo,string branchId,string? warehouseId,string? pointOfSaleId,DateTime expiresAt)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"]!;
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, userId),
        new(JwtRegisteredClaimNames.UniqueName, userId),
        new(ClaimTypes.NameIdentifier, userId),
        new(ClaimTypes.Name, userId),
        new("is_admin", isAdmin.ToString().ToLower()),
        new("company_id", EmpCodigo.ToString()),
        new("branch_id", branchId),
        new("token_type", "context")
    };

        if (!string.IsNullOrWhiteSpace(warehouseId))
            claims.Add(new Claim("warehouse_id", warehouseId));

        if (!string.IsNullOrWhiteSpace(pointOfSaleId))
            claims.Add(new Claim("point_of_sale_id", pointOfSaleId));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}