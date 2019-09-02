using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BasicWebAPI.Models.Entity;
using BasicWebAPI.Models.Param;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BasicWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        public AuthController(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<object> Login([FromBody] LoginParam loginParam)
        {
            var result = await signInManager.PasswordSignInAsync(loginParam.UserName, loginParam.Password, false, false);
            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(loginParam.UserName);
                return await generateToken(user);
            }
            throw new ApplicationException();
        }

        [HttpPost("register")]
        public async Task<object> Register([FromBody] RegisterParam registerParam)
        {
            var user = new ApplicationUser 
            {
                UserName = registerParam.UserName
            };
            var result = await userManager.CreateAsync(user, registerParam.Password);
            if (result.Succeeded)
            {
                var isRoleExist = await roleManager.RoleExistsAsync("Admin");
                if(!isRoleExist)
                {
                    result = await roleManager.CreateAsync(new IdentityRole 
                        {
                            Name = "Admin"
                        }
                    );
                }

                if(!result.Succeeded)
                    throw new ApplicationException($"Role creation failed, reason: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                result = await userManager.AddToRoleAsync(user, "Admin");
                if(result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return await generateToken(user);
                }
            }
            throw new ApplicationException($"Register failed, reason: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        private async Task<object> generateToken(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            if(roles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.FirstOrDefault()));
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var expires = DateTime.Now.AddHours(12);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtIssuer"],
                audience: configuration["JwtIssuer"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}