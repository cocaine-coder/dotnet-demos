using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Demo_Jwt.Services;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims);

    IEnumerable<Claim>? ResolveToken(string token);
}

public class JwtService : IJwtService
{
    private readonly SecurityJwtConfig jwtConfig = new ();
    public JwtService(IConfiguration configuration)
    {
        configuration.GetSection("Security:Jwt").Bind(jwtConfig);
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
        issuer: jwtConfig.Issuer,
        audience: jwtConfig.Audience,
        expires: DateTime.Now.AddMinutes(jwtConfig.ExpireMinutes),
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(jwtConfig.KeyBytes),
            SecurityAlgorithms.HmacSha256),
        claims: claims
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public IEnumerable<Claim>? ResolveToken(string token)=>
        new JwtSecurityTokenHandler().ReadJwtToken(token)?.Claims;
}
