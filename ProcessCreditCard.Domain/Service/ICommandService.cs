using ProcessCreditCard.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCreditCard.Domain.Service
{
    public interface ICommandService
    {
        Task<List<Data.Entity.Command>> ListCommands(string CardHolderName);
        Task<string> AddCommand(Command command);
    }
}
