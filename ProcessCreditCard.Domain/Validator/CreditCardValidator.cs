using FluentValidation;
using ProcessCreditCard.Domain.Model;
using ProcessCreditCard.Domain.Service;
using System;

namespace ProcessCreditCard.Domain.Validator
{
    public class CreditCardValidator : AbstractValidator<AddCreditCard>
    {
        private ICreditCardService CreditCardService;
        public CreditCardValidator(ICreditCardService CreditCardService)
        {
            this.CreditCardService = CreditCardService;
            RuleFor(a => a.CardHolderName)
                    .NotEmpty()
                    .WithMessage("CardHolder Name is Required")
                    .MaximumLength(25)
                    .WithMessage("Maximum Length of the Name is Limited to 30 Characters")
                    .Custom((name, context) =>
                    {
                        var creditCardData = this.CreditCardService.GetCreditCardInfoAsync(name);
                        if (creditCardData != null)
                            context.AddFailure($"User cannot have more than one CreditCard. Cannot issue another Credit Card to {name}.");
                    });
            RuleFor(a => a.CardNumber)
                   .NotEmpty()
                   .WithMessage("Credit Card Number is Required")
                   .MaximumLength(16)
                   .WithMessage("Card Number can't be more than 16 Characters")
                   .Custom((cardNumber, context) =>
                    {
                        if (!CheckLuhnAlgorithm(cardNumber))
                            context.AddFailure($"{cardNumber} is Not a Valid CardNumber ");
                    });
            RuleFor(a => a.CreditLimit)
                   .NotEmpty()
                   .WithMessage("Credit Limit is Required")
                   .LessThanOrEqualTo(10000)
                   .WithMessage($"Card Limit should be less than $10000.");
        }


        /// <summary>
        ///     Validates Card Number Against the Luhn Algorith
        /// </summary>
        /// <param name="CreditCardNumber"> The Card Number Input </param>
        /// <returns> True / False </returns>
        private static bool CheckLuhnAlgorithm(string CreditCardNumber)
        {
            int nDigits = CreditCardNumber.Length;

            int nSum = 0;
            bool isSecond = false;
            for (int i = nDigits - 1; i >= 0; i--)
            {
                int cardNumberDigit = Convert.ToInt32(CreditCardNumber[i]);

                if (isSecond == true)
                    cardNumberDigit *= 2;

                // We add two digits to handle
                // cases that make two digits
                // after doubling
                nSum += cardNumberDigit / 10;
                nSum += cardNumberDigit % 10;

                isSecond = !isSecond;
            }
            return (nSum % 10 == 0);
        }
    }
}
