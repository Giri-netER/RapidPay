using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using RapidPay.Controllers;
using RapidPay.Models;
using RapidPay.Services;
using Xunit;
namespace RapidPay.Api.IntegrationTests.ControllerTests
{
    public class AuthControllerIntegrationTests : IDisposable
	{

        private AuthController _controller;
        private ApplicationDbContext _context;

        public AuthControllerIntegrationTests()
        {
            // Configure in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed test data
            _context.Users.Add(new User { UserName ="test", Email = "test@example.com", Password = "password" });
            _context.SaveChanges();

            // Mock IConfiguration
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            config["JwtOptions:SigningKey"] = "ABCDeuejjysnfkebn@!!!&whfbwjwbcm@#jjk1389248BUE*@H@Nz#*@Y#KJNMBUYDF&^UT";
            config["JwtOptions:Issuer"] = "test_issuer";
            config["JwtOptions:Audience"] = "test_audience";
            var logger = new NullLogger<AuthController>();
            _controller = new AuthController(config, _context,logger);
        }

        [Fact]
        public void Login_ValidUser_ReturnsToken()
        {
            var user = new User { Email = "test@example.com", Password = "password" };

            var result = _controller.Login(user);

            Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
            var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            Assert.NotNull(okResult.Value);
            Assert.NotNull(okResult.Value.GetType().GetProperty("token"));
        }

        [Fact]
        public void Login_InvalidUser_ReturnsUnauthorized()
        {
            var user = new User { Email = "invalid@example.com", Password = "password" };

            var result = _controller.Login(user);

            Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedResult>(result);
        }

        [Fact]
        public void Register_ValidUser_ReturnsOk()
        {
            var user = new User {  UserName = "test",Email = "newuser@example.com", Password = "password" };

            var result = _controller.Register(user);

            Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        }

        [Fact]
        public void Register_DuplicateUser_ReturnsConflict()
        {
            var user = new User { Email = "test@example.com", Password = "password" };

            var result = _controller.Register(user);

            Assert.IsType<Microsoft.AspNetCore.Mvc.ConflictObjectResult>(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}