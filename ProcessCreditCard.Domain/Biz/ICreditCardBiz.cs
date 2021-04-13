using ProcessCreditCard.Data.Entity;
using ProcessCreditCard.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessCreditCard.Domain.Biz
{
    public interface ICreditCardBiz
    {
        Task<List<string>> ExecuteCommands(List<string> inputCommands);

        Task<string> AddCreditCardAsync(AddCreditCard creditCard);

        Task<List<CreditCard>> ListCreditCardsAsync();

        Task<CreditCard> GetCreditCardAsync(string cardholderName);

        Task<string> ChargeCreditCardAsync(CreditCardActions creditCardActions);

        Task<string> AddCashBackAsync(CreditCardActions creditCardActions);



        Task<List<Command>> ListCommands(string cardHolderName);

    }
}
