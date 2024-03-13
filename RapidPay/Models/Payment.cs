using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public double Amount { get; set; }
        public double Fee { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
