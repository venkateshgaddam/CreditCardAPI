using ProcessCreditCard.Data.Entity;
using ProcessCreditCard.Data.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessCreditCard.Domain.Service
{
    public class CommandService : ICommandService
    {
        private readonly IGenericRepository<Command> genericRepository;

        public CommandService(IGenericRepository<Command> genericRepository)
        {
            this.genericRepository = genericRepository;
        }


        public async Task<string> AddCommand(Command command)
        {
            await this.genericRepository.AddAsync(command);
            return "success";
        }

        public async Task<List<Command>> ListCommands(string CardHolderName)
        {
            var result = await this.genericRepository.GetAllAsync();
            return result.Where(a => a.CardHolderName == CardHolderName).ToList();
        }
    }
}
