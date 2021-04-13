using Microsoft.EntityFrameworkCore;
using ProcessCreditCard.Data.Entity;

namespace ProcessCreditCard.Data.EFCore
{
    public class CreditCardContext : DbContext
    {
        public CreditCardContext(DbContextOptions<CreditCardContext> options) : base(options)
        { }

        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
