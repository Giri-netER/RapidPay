using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RapidPay.Models;
using RapidPay.Services;

namespace RapidPay.Controllers
{

    [Route("api/v1/auth")]
    public class AuthController : Controller
    {

        private IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _Logger;

        public AuthController(IConfiguration configuration, ApplicationDbContext context, ILogger<AuthController> logger)
        {
            _config = configuration;
            _context = context;
            _Logger = logger;
        }

        private User AuthenticateUser(User user)
        {
            var dbUser = _context.Users.SingleOrDefault(u => u.Email == user.Email);

            // If user  with the provided email exists
            if (dbUser != null)
            {
                // Verify if the password matches
                if (dbUser.Password == user.Password)
                {
                    // Passwords match, return the authenticated user
                    return dbUser;
                }
            }

            // Either user does not exist or password does not match
            return null;
        }

        private string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtOptions:SigningKey"]));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["JwtOptions:Issuer"], _config["JwtOptions:Audience"], null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            IActionResult response = Unauthorized();
            var authenticatedUser = AuthenticateUser(user);
            if (authenticatedUser != null)
            {
                var token = GenerateToken();
                response = Ok(new { token = token });
                _Logger.LogInformation("User logged in: {Email}", user.Email);
            }
            else
            {
                _Logger.LogWarning("Login attempt failed for email: {Email}", user.Email);
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                _Logger.LogError("Invalid model state while registering user");
                return BadRequest(ModelState);
            }

            // Check if the email is already registered
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                _Logger.LogWarning("Registration failed - Email already exists: {Email}", user.Email);
                return Conflict("Email already exists");
            }

            // Add the user to the database
            _context.Users.Add(user);
            _context.SaveChanges();
            _Logger.LogInformation("User registered successfully: {Email}", user.Email);

            return Ok("User registered successfully");
        }
    }
}

