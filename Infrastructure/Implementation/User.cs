using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Implementation
{
    public class User : IUser
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration configuration;

        public User(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
        }

        private async Task<ApplicationUser> FindUserByEmail(string email)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDto)
        {
            var user = await FindUserByEmail(loginDto.Email!);
            if (user == null)
            {
                return new LoginResponse(false, "User not found");
            }
            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password,user.Password);
            if (checkPassword)
            {
                return new LoginResponse(true, "Login Successfully", GenerateJWTToken(user));
            }
            else 
            {
                return new LoginResponse(false,"Invalid Credentials");
            }
        }

        private string GenerateJWTToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),


            };
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Bearer"],
                audience: configuration["Jwt:audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUser)
        {
            var user = await FindUserByEmail(registerUser.Email!);
            if( user != null)
            {
                return new RegistrationResponse(false, "User already exists" );
            }
            appDbContext.Users.Add(new ApplicationUser()
            {
                Name = registerUser.Name,
                Email = registerUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password)
            });
            await appDbContext.SaveChangesAsync();
            return new RegistrationResponse(true,"Registration Successfull");
        }
    }
}
