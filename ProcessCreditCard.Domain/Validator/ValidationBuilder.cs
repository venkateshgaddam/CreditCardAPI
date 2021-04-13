using ProcessCreditCard.Domain.Service;

namespace ProcessCreditCard.Domain.Validator
{
    public class ValidationBuilder : IValidationBuilder
    {
        public bool isValid { get; set; }

        public string ErrorMessage { get; set; }

        private ICreditCardService creditCardService;
        public ValidationBuilder(ICreditCardService creditCardService)
        {
            this.creditCardService = creditCardService;
            isValid = true;
        }

        public bool ValidateCardLimit(decimal CreditLimit, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (CreditLimit > 10000)
            {
                errorMessage = $"InvalidCredit Limit {CreditLimit}";
                return false;
            }
            return true;
        }

        public bool ValidateCardName(string CardHolderName, out string errorMessage)
        {
            errorMessage = string.Empty;
            var creditCardData = creditCardService.GetCreditCardInfoAsync(CardHolderName).Result;
            if (creditCardData != null)
            {
                errorMessage = $"User cannot have more than one CreditCard. Cannot issue another Credit Card to {CardHolderName}.";
                return false;
            }
            return true;
        }

        public bool ValidateCardNumber(string CreditCardNumber, out string errorMessage)
        {
            int nDigits = CreditCardNumber.Length;
            errorMessage = string.Empty;
            int nSum = 0;
            bool isSecond = false;
            for (int i = nDigits - 1; i >= 0; i--)
            {
                int cardNumberDigit = CreditCardNumber[i] - '0';

                if (isSecond == true)
                    cardNumberDigit *= 2;

                // We add two digits to handle
                // cases that make two digits
                // after doubling
                nSum += cardNumberDigit / 10;
                nSum += cardNumberDigit % 10;

                isSecond = !isSecond;
            }
            if (nSum % 10 != 0)
            {
                isValid = false;
                errorMessage = $"Invalid Card Number {CreditCardNumber}";
                return false;
            }
            return true;
        }
    }

    public interface IValidationBuilder
    {
        public bool ValidateCardNumber(string CardNumber, out string errorMessage);
        public bool ValidateCardName(string CardNumber, out string errorMessage);
        public bool ValidateCardLimit(decimal CreditLimit, out string errorMessage);

    }
}
