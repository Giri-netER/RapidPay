using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.Models
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long  Id { get; set; }

        
        
        public string CardNumber { get; set; }

        public double Balance { get; set; }


        public Card(double balance)
        {
            CardNumber = GenerateRandomDigits(15);
            Balance = balance;
        }
        

        private string GenerateRandomDigits(int length)
        {
            Random random = new Random();
            string digits = "";
            for (int i = 0; i < length; i++)
            {
                digits += random.Next(0, 10);
            }
            return digits;
        }
    }
}
