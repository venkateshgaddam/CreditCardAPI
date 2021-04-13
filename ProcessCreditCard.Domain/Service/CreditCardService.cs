using ProcessCreditCard.Data.Entity;
using ProcessCreditCard.Data.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessCreditCard.Domain.Service
{
    public class CreditCardService : ICreditCardService
    {
        private readonly IGenericRepository<CreditCard> genericRepository;
        public CreditCardService(IGenericRepository<CreditCard> genericRepository)
        {
            this.genericRepository = genericRepository;
        }

        public async Task<string> AddCashBackAsync(CreditCard creditCardActions)
        {
            await genericRepository.UpdateAsync(creditCardActions);
            return "success";
        }

        public async Task<string> AddCreditCardAsync(CreditCard creditCard)
        {
            await genericRepository.AddAsync(creditCard);
            return "success";
        }

        public async Task<string> ChargeCreditCardAsync(CreditCard creditCardActions)
        {
            await genericRepository.UpdateAsync(creditCardActions);
            return "success";
        }

        public async Task<CreditCard> GetCreditCardInfoAsync(string CardHolderName)
        {
            var result = await genericRepository.GetAllAsync();
            return result.FirstOrDefault(a => a.CardHolderName == CardHolderName);
        }

        public async Task<IQueryable<CreditCard>> ListCreditCardsAsync()
        {
            return await genericRepository.GetAllAsync();
        }
    }
}
