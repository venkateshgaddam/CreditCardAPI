using Microsoft.EntityFrameworkCore;
using ProcessCreditCard.Data.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProcessCreditCard.Data.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private CreditCardContext creditCardContext;
        private bool _disposed;

        public GenericRepository(CreditCardContext creditCardContext)
        {
            this.creditCardContext = creditCardContext;
        }

        public async Task AddAsync(TEntity entity)
        {
            await creditCardContext.Set<TEntity>().AddAsync(entity);
            await creditCardContext.SaveChangesAsync();
            creditCardContext.Entry(entity).State = EntityState.Detached;
        }

        
        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            var dbEntity = creditCardContext.Set<TEntity>();
            var resultSet = dbEntity.AsNoTracking();
            return resultSet;
        }

        public async Task<TEntity> GetAsync(string CardHolderName)
        {
            TEntity result = await creditCardContext.Set<TEntity>().FindAsync(CardHolderName);
            return result;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            creditCardContext.Set<TEntity>().Update(entity);
            await creditCardContext.SaveChangesAsync();
            creditCardContext.Entry(entity).State = EntityState.Detached;
        }
    }
}
