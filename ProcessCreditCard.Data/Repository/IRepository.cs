using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessCreditCard.Data.Repository
{
    public interface IRepository<T>
    {
        Task<IQueryable<T>> GetAllAsync();

        Task<T> GetAsync(string CardHolderName);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);
    }
}
