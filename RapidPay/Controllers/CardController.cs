using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidPay.Models;
using RapidPay.Services;

namespace RapidPayAPI.Controllers
{
    [ApiController]
    [Route("api/v1/card")]
    public class CardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UniversalFeesExchange _feesExchange;
        private readonly ILogger<CardController> _logger;

        public CardController(ApplicationDbContext context, UniversalFeesExchange feesExchange, ILogger<CardController> logger)
        {
            _context = context;
            _feesExchange = feesExchange;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCard([FromBody] Card request)
        {
            try
            {
                var card = new Card(request.Balance);

                _context.Cards.Add(card);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Card Creation Completed Successfully");
                return Ok(card);
            }
            catch(Exception ex)
            { 
                _logger.LogError(ex, "Error occurred while creating a card"); // Log error
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [Authorize]
        [HttpPost("pay")]
        public async Task<IActionResult> Pay(string cardNumber, double amount)
        {
            try
            {
                var card = await _context.Cards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
                if (card == null)
                    return NotFound("Card not found");

                if (card.Balance < amount)
                    return BadRequest("Insufficient balance");

                double fee = _feesExchange.GetCurrentFee();
                double totalAmount = amount + fee;

                if (card.Balance < totalAmount)
                    return BadRequest("Insufficient balance including fee");

                card.Balance -= totalAmount;

                // Store the payment
                var payment = new Payment
                {
                    CardNumber = cardNumber,
                    Amount = amount,
                    Fee = fee,
                    Timestamp = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Payment Completed Successfully");
                return Ok($"Paid {amount} from card {cardNumber}. Fee: {fee}. New balance: {card.Balance}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while making a payment"); // Log error
                return StatusCode(500, "An error occurred while processing your request");
            }

        }

        [Authorize]
        [HttpGet("{cardNumber}/balance")]
        public async Task<IActionResult> GetBalance(string  cardNumber)
        {
            try
            {
                var card = await _context.Cards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
                if (card == null)
                    return NotFound("Card not found");

                _logger.LogInformation("Card Details Fetched Successfully");
                return Ok($"Card {cardNumber} balance: {card.Balance}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching card details"); // Log error
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
