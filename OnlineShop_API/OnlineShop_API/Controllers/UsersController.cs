﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop_API.Data;
using OnlineShop_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OnlineShop_API.Dto;
using OnlineShop_API.DTOs;
using OnlineShop_API.Identity;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShop_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly OnlineShopDbContext _context;
        private readonly IConfiguration _config;

        public UsersController(OnlineShopDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Users
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = HashPassword(registerDto.Password), // Hasha lösenordet
                Address = registerDto.Address,
                Mobile = registerDto.Mobile,
                City = registerDto.City,
                Zipcode = registerDto.Zipcode,
                IsAdmin = false // Se till att användaren inte kan sätta sig själv som admin
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);

            // Kontrollera att användaren existerar och lösenordet stämmer
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            if (!VerifyPassword(user.Password, loginDto.Password))
            {
                return Unauthorized("Invalid password.");
            }

            // Generera JWT-token för användaren
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        // POST: api/users/logout
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Eftersom JWT inte lagras server-side, görs ingenting här
            return Ok("User logged out successfully.");
        }

        // GET: api/users
        [HttpGet]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync(); // Hämtar alla användare från databasen
            return Ok(users); // Returnerar användarna som JSON
        }

        [HttpPut("profile")]
        [Authorize]
        public IActionResult UpdateProfile([FromBody] UserDto updatedUser)
        {
            if (updatedUser == null)
            {
                return BadRequest("User data is required."); // Returnera 400 om ingen data skickas
            }

            // Hämta användarens ID från claims
            var userId = GetCurrentUserId();

            // Hämta användaren med det angivna ID:t
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound(); // Returnera 404 om användaren inte hittas
            }

            // Uppdatera användarens information utan att ändra lösenordet
            user.Name = updatedUser.Name;
            user.Address = updatedUser.Address;
            user.Mobile = updatedUser.Mobile;
            user.City = updatedUser.City;
            user.Zipcode = updatedUser.Zipcode;

            _context.SaveChanges(); // Spara ändringarna i databasen
            return Ok("Profile updated successfully."); // Returnera framgångsmeddelande
        }
        [HttpPut("change-password")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            // Kontrollera att DTO:n inte är null
            if (changePasswordDto == null)
            {
                return BadRequest("Change password data is required.");
            }

            // Hämta användarens ID från claims
            var userId = GetCurrentUserId();

            // Hämta användaren med det angivna ID:t
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found."); // Returnera 404 om användaren inte hittas
            }

            // Kontrollera att det gamla lösenordet är korrekt
            if (!VerifyPassword(user.Password, changePasswordDto.OldPassword))
            {
                return BadRequest("Old password is incorrect."); // Returnera 400 om det gamla lösenordet är felaktigt
            }

            // Hasha det nya lösenordet
            user.Password = HashPassword(changePasswordDto.NewPassword);
            _context.SaveChanges(); // Spara ändringarna i databasen

            return Ok("Password changed successfully."); // Returnera framgångsmeddelande
        }


        // GET: api/users/orders
        [HttpGet("orders")]
        [Authorize]
        public IActionResult GetUserOrders()
        {
            var userId = GetCurrentUserId();
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems) // Inkludera orderartiklar
                .ToList();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }


        [HttpGet("profile")]
        [Authorize]  // Endast inloggade användare kan komma åt denna
        public async Task<ActionResult<UserDto>> GetUserProfile()
        {
            // Hämta användarens ID från token (NameIdentifier)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Om användarens ID inte finns i token, returnera 401
            }

            // Försök att konvertera userId till en int
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("Invalid user ID in token.");
            }

            // Hämta användaren från databasen baserat på användarens ID (nu som int)
            var user = await _context.Users.FindAsync(parsedUserId);
            if (user == null)
            {
                return NotFound(); // Om användaren inte finns, returnera 404
            }

            // Mappa användardata från User till UserDto
            var userDto = new UserDto
            {
                Name = user.Name,
                Address = user.Address,
                Mobile = user.Mobile,
                City = user.City,
                Zipcode = user.Zipcode,
                IsAdmin = user.IsAdmin
            };

            // Returnera den mappade användardatan
            return Ok(userDto);
        }



        // Helper methods
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string hashedPassword, string password)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedPassword == hashedInputPassword;
        }

        private string GenerateJwtToken(Users user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Användarens ID
        new Claim(ClaimTypes.Email, user.Email) // Användarens e-post
    };

            // Lägg till ett claim för admin om användaren är admin
            if (user.IsAdmin)
            {
                claims.Add(new Claim(IdentityData.AdminUserClaimName, "true"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
