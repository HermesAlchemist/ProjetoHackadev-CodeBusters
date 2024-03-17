﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PayWiseBackend.Domain.Context;
using PayWiseBackend.Domain.DTOs;
using PayWiseBackend.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PayWiseBackend.Services;

public class AuthService : IAuthService
{
    private readonly PaywiseDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(PaywiseDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public string GenerateAccessToken(int clienteId)
    {
        string accessToken = GenerateToken(clienteId, TokenType.Access);
        return accessToken;
    }

    public string GenerateRefreshToken(int clienteId)
    {
        string refreshToken = GenerateToken(clienteId, TokenType.Refresh);
        return refreshToken;
    }

    public string GenerateToken(int clienteId, TokenType type)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, clienteId.ToString())
            }),
            Expires = type == TokenType.Refresh ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(10),
            Issuer = _config["Jwt:issuer"],
            Audience = _config["Jwt:audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<Cliente?> ValidateCredentials(LoginRequestDTO loginCredentials)
    {
        var cliente = await _context.Clientes.SingleOrDefaultAsync(cliente => cliente.Cpf == loginCredentials.Cpf);

        if (cliente is null)
        {
            return null;
        }

        bool password = BCrypt.Net.BCrypt.Verify(loginCredentials.Senha, cliente.Senha);

        if (!password)
        {
            return null;
        }

        return cliente;
    }


}