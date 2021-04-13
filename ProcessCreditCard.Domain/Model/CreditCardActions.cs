using System.ComponentModel.DataAnnotations;

namespace ProcessCreditCard.Domain.Model
{
    public class CreditCardActions
    {
        [Required]
        public string CommandType { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string CardHolderName { get; set; }
    }
}
