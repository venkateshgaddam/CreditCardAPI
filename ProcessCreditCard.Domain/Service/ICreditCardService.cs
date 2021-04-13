using ProcessCreditCard.Data.Entity;
using ProcessCreditCard.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessCreditCard.Domain.Service
{
    public interface ICreditCardService
    {
        Task<string> AddCreditCardAsync(CreditCard creditCard);

        Task<IQueryable<CreditCard>> ListCreditCardsAsync();

        Task<CreditCard> GetCreditCardInfoAsync(string CardHolderName);

        Task<string> ChargeCreditCardAsync(CreditCard creditCardActions);

        Task<string> AddCashBackAsync(CreditCard creditCardActions);
    }
}
