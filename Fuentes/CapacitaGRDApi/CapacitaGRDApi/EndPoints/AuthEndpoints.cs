using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class AuthEndpoints
    {
        public static RouteGroupBuilder MapAuth(this RouteGroupBuilder group)
        {
            group.MapPost("/login", Login);

            return group;
        }

        static async Task<Results<Ok<AuthResponseDTO>, UnauthorizedHttpResult>> Login(
                   LoginRequestDTO loginRequest,
                   IConfiguration configuration,
                   IRepositorioUsuarios repositorioUsuarios
               )
        {
            // Validar credenciales del usuario
            var usuario = await repositorioUsuarios.Busqueda(loginRequest.Username, loginRequest.Password);

            var usuarioValido = usuario.FirstOrDefault();
            if (usuarioValido == null)
            {
                return TypedResults.Unauthorized(); // Si no se encuentra el usuario, devolvemos un error Unauthorized
            }

            // Verificar que la configuración de JWT no sea nula
            


            // Crear claims para el token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,  usuarioValido.USUARIO),
                new Claim(ClaimTypes.Role, usuarioValido.ROL),
                new Claim("UserId",  usuarioValido.ID_USUARIO.ToString()),
                //new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeSeconds().ToString())  // Añadimos la expiración del token

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Configurar los parámetros del token
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow, // Hora de inicio
                expires: DateTime.UtcNow.AddDays(1), //AddHours(1), Hora de expiración en UTC
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            string tokenString = tokenHandler.WriteToken(token);


            var authResponse = new AuthResponseDTO
            {
                Token = tokenString,
                Expiration = token.ValidTo
            };

            return TypedResults.Ok(authResponse);
        }

    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


}
