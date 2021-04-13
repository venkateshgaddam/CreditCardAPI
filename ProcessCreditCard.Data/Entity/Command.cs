using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCreditCard.Data.Entity
{
    public class Command
    {
        public Guid CommandId { get; set; }
        public string CreditCommand { get; set; }
        public string CardHolderName { get; set; }
    }
}
