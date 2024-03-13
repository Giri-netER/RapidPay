using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidPay.Models;
using RapidPay.Services;
using Serilog;
using RapidPayAPI.Controllers;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RapidPay.Api.IntegrationTests.ControllerTests
{
    public class CardControllerIntegrationTests : IDisposable
    {
        private CardController _controller;
        private ApplicationDbContext _context;
        private Card _testCard;


        public CardControllerIntegrationTests()
        {
            // Configure in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            var logger = new NullLogger<CardController>();

            _testCard = new Card(300.0);


            _testCard.CardNumber = "123456789123456";

            _context.Cards.Add(_testCard);
            _context.SaveChanges();


            // Mock UniversalFeesExchange
            var feesExchange = new UniversalFeesExchange();

            _controller = new CardController(_context, feesExchange, logger);
        }

        [Fact]
        public async Task CreateCard_ValidRequest_ReturnsOk()
        {
            var cardRequest = new Card(200.0);

            var result = await _controller.CreateCard(cardRequest);

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var card = okResult.Value as Card;
            Assert.NotNull(card);
            Assert.Equal(cardRequest.Balance, card.Balance);
        }

        [Fact]
        public async Task Pay_InsufficientBalance_ReturnsBadRequest()
        {   
            var amount = 300.0;
             
            var result = await _controller.Pay(_testCard.CardNumber, amount);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetBalance_ExistingCard_ReturnsOkWithBalance()
        {
            var result = await _controller.GetBalance(_testCard.CardNumber);

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult.Value);
            Assert.Equal($"Card {_testCard.CardNumber} balance: 300", okResult.Value.ToString());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


