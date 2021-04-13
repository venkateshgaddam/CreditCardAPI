using System;

namespace ProcessCreditCard.Data.Entity
{
    public class CreditCard
    {
        public Guid CreditCardId { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public decimal TotalCreditLimit { get; set; }

        /// <summary>
        /// The Limit is assumed to be in Dollars
        /// </summary>
        public decimal Balance { get; set; } 

    }
}
