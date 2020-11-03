using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Api.Domain.Dtos;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Services.User;
using Api.Domain.Repository;
using Api.Domain.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Service.Services
{
  public class LoginService : ILoginService
  {
    private IUserRepository _repository;
    private SigningConfigurations _signingConfigurations;
    private TokenConfigurations _tokenConfigurations;
    private IConfiguration _configuration { get; }

    public LoginService(
      IUserRepository repository,
      SigningConfigurations signingConfigurations,
      TokenConfigurations tokenConfigurations,
      IConfiguration configuration)
    {
      _repository = repository;
      _signingConfigurations = signingConfigurations;
      _tokenConfigurations = tokenConfigurations;
      _configuration = configuration;
    }

    public async Task<object> FindByLogin(LoginDto user)
    {
      var baseUser = new UserEntity();

      if (user != null && !string.IsNullOrWhiteSpace(user.Email))
      {
        baseUser = await _repository.FindByLogin(user.Email);

        if (baseUser == null)
        {
          return new
          {
            authenticate = false,
            message = "Falha ao autenticar"
          };
        }
        else
        {
          var identity = new ClaimsIdentity(
            new GenericIdentity(user.Email),
            new[]
            {
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
            }
          );

          DateTime createdDate = DateTime.Now;
          DateTime expirationDate = createdDate + TimeSpan.FromSeconds(_tokenConfigurations.Seconds); // 60 segundos = 1min

          var handler = new JwtSecurityTokenHandler();
          string token = CreateToken(identity, createdDate, expirationDate, handler);

          return SuccessObject(createdDate, expirationDate, token, baseUser);
        }
      }
      else
      {
        return new
        {
          authenticate = false,
          message = "Falha ao autenticar"
        };
      }
    }

    private string CreateToken(ClaimsIdentity identity, DateTime createdDate, DateTime expirationDate, JwtSecurityTokenHandler handler)
    {
      var securityToken = handler.CreateToken(new SecurityTokenDescriptor
      {
        Issuer = _tokenConfigurations.Issuer,
        Audience = _tokenConfigurations.Audience,
        SigningCredentials = _signingConfigurations.SigningCredentials,
        Subject = identity,
        NotBefore = createdDate,
        Expires = expirationDate,
      });

      var token = handler.WriteToken(securityToken);

      return token;
    }

    private object SuccessObject(DateTime createdDate, DateTime expirationDate, string token, UserEntity user)
    {
      return new
      {
        authenticated = true,
        created = createdDate.ToString("yyyy-MM-dd HH:mm:ss"),
        expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
        accessToken = token,
        email = user.Email,
        name = user.Name,
        message = "Usuário autenticado com sucesso"
      };
    }
  }
}