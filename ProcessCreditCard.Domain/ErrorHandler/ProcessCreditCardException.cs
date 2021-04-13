using System;

namespace ProcessCreditCard.Domain.ErrorHandler
{
    public class ProcessCreditCardException : Exception
    {
        public ProcessCreditCardException(string errorMessage) : base(errorMessage) { }

        public ProcessCreditCardException(string errorMessage, Exception exception) : base(errorMessage, exception) { }
    }
}
