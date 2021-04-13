using Swashbuckle.AspNetCore.Filters;

namespace ProcessCreditCard.Domain.Model
{
    public class AddCreditCard
    {
        public string CardHolderName { get; set; }

        public decimal CreditLimit { get; set; }

        public string CardNumber { get; set; }

    }


    public class AddCreditCardExample : IExamplesProvider<AddCreditCard>
    {
        public AddCreditCard GetExamples()
        {
            return new AddCreditCard
            {
                CardHolderName = "Alice",
                CardNumber = "79927398713",
                CreditLimit = 10000
            };
        }
    }



    //return new List<string>
    //        {
    //            "Add Alice 4111111111111111 $1000",
    //            "Add Bob 5454545454545454 $3000",
    //            "Add Carl 1234567890123456 $2000",
    //            "Charge Alice $500",
    //            "Charge Alice $800",
    //            "Charge Bob $7",
    //            "Credit Bob $100",
    //            "Credit Carl $200"
    //        };
}
