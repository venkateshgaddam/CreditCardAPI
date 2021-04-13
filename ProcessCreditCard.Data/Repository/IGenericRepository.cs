using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCreditCard.Data.Repository
{
    public interface IGenericRepository<TEntity> :  IRepository<TEntity> where TEntity : class
    {
    }
}
