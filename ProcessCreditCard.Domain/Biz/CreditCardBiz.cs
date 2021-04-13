using AutoMapper;
using Microsoft.Extensions.Logging;
using ProcessCreditCard.Data.Entity;
using ProcessCreditCard.Domain.ErrorHandler;
using ProcessCreditCard.Domain.Model;
using ProcessCreditCard.Domain.Service;
using ProcessCreditCard.Domain.Validator;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ProcessCreditCard.Domain.Biz
{
    public class CreditCardBiz : ICreditCardBiz
    {
        private readonly ICreditCardService creditCardService;
        private readonly IValidationBuilder validationBuilder;
        private readonly IMapper mapper;
        private readonly ILogger<CreditCardBiz> logger;
        private readonly ICommandService commandService;
        private List<string> ErrorMessages { get; set; }

        public CreditCardBiz(ICreditCardService creditCardService, IValidationBuilder validationBuilder, ILogger<CreditCardBiz> logger, ICommandService commandService)
        {
            this.creditCardService = creditCardService;
            this.validationBuilder = validationBuilder;
            this.commandService = commandService;
            this.logger = logger;
            var config = new MapperConfiguration(a => { a.CreateMap<AddCreditCard, CreditCard>(); });
            mapper = config.CreateMapper();
            ErrorMessages = new List<string>();
        }

        public async Task<string> AddCashBackAsync(CreditCardActions creditCardActions)
        {
            logger.LogDebug($"Within {nameof(AddCashBackAsync)} method.");
            logger.LogDebug($"Check the Card Details first.");

            CreditCard creditCard = await creditCardService.GetCreditCardInfoAsync(creditCardActions.CardHolderName);

            if (creditCard == null)
            {
                ErrorMessages.Add($"{creditCardActions.CardHolderName}:No Credit Card Exists for {creditCardActions.CardHolderName}");
                return $"No Credit Card Exists for {creditCardActions.CardHolderName}";
            }

            decimal currentBalance = creditCard.Balance;
            if (currentBalance != 0)
            {
                creditCard.Balance -= creditCardActions.Amount;
            }
            else
            {
                ErrorMessages.Add($"{creditCardActions.CardHolderName} : Cannot add Credits without Any Charges on this Card {creditCard.CardNumber} with Balance {creditCard.Balance}");
                return $"{creditCardActions.CardHolderName} : Cannot add Credits without Any Charges on this Card {creditCard.CardNumber} with Balance {creditCard.Balance}";
            }

            string response = await creditCardService.ChargeCreditCardAsync(creditCard);

            if (response == "success")
            {
                
                logger.LogInformation($"Credit Card Added Successfully");
                return $"{creditCard.CardHolderName}:${creditCard.Balance}";
            }
            else
            {
                return $"{creditCard.CardHolderName}:Something went wrong while Charging the CreditCard {creditCard.CardNumber} with Amount {creditCardActions.Amount}";
            }
        }

        public async Task<string> AddCreditCardAsync(AddCreditCard addCreditCardModel)
        {
            logger.LogDebug($"Within {nameof(AddCreditCardAsync)} method.");
            var ValidLimit = this.validationBuilder.ValidateCardLimit(addCreditCardModel.CreditLimit, out string errorMessage);
            if (!ValidLimit)
            {
                ErrorMessages.Add($"{addCreditCardModel.CardHolderName}:{errorMessage}");
                return $"{addCreditCardModel.CardHolderName}:{errorMessage}";
            }

            var ValidateCardHolderName = this.validationBuilder.ValidateCardName(addCreditCardModel.CardHolderName, out string InValidCardHolderError);
            if (!ValidateCardHolderName)
            {
                ErrorMessages.Add($"{addCreditCardModel.CardHolderName}:{InValidCardHolderError}");
                return $"{addCreditCardModel.CardHolderName}:{InValidCardHolderError}";
            }

            var ValidateCardNumber = this.validationBuilder.ValidateCardNumber(addCreditCardModel.CardNumber, out string InValidCardNumberError);
            if (!ValidateCardNumber)
            {
                ErrorMessages.Add($"{addCreditCardModel.CardHolderName}:{InValidCardNumberError}");
                return $"{addCreditCardModel.CardHolderName}:{InValidCardNumberError}";
            }

            logger.LogDebug($"Validation is Successfull");
            CreditCard dbObject = mapper.Map<CreditCard>(addCreditCardModel);
            dbObject.TotalCreditLimit = addCreditCardModel.CreditLimit;

            logger.LogInformation($"Created DbObject for Creating a new CreditCard");
            logger.LogInformation($"All Validations were Succeeded");

            string response = await creditCardService.AddCreditCardAsync(dbObject);

            if (response == "success")
            {
                logger.LogInformation($"Credit Card Added Successfully");
                return $"{dbObject.CardHolderName}:${dbObject.Balance}";
            }
            return $"{addCreditCardModel.CardHolderName}:An Error occured while processing the request";
        }

        public async Task<string> ChargeCreditCardAsync(CreditCardActions creditCardActions)
        {
            logger.LogDebug($"Within {nameof(ChargeCreditCardAsync)} method.");
            logger.LogDebug($"Check the Card Details first.");

            CreditCard creditCard = await creditCardService.GetCreditCardInfoAsync(creditCardActions.CardHolderName);

            if (creditCard == null)
            {
                ErrorMessages.Add($"{creditCardActions.CardHolderName}:No Credit Card Exists for {creditCardActions.CardHolderName}");
                return "No Credit Card Exists with the Given Data";
            }

            decimal currentAvailableLimit = creditCard.TotalCreditLimit;

            if (creditCardActions.Amount < currentAvailableLimit)
            {
                creditCard.Balance += creditCardActions.Amount;
                creditCard.TotalCreditLimit -= creditCardActions.Amount;
                string response = await creditCardService.ChargeCreditCardAsync(creditCard);

                if (response == "success")
                {
                    logger.LogInformation($"Credit Card Added Successfully");
                    return $"{creditCard.CardHolderName}:${creditCard.Balance}";
                }
                else
                {
                    ErrorMessages.Add($"{creditCard.CardHolderName}: Something went wrong while Charging the CreditCard {creditCard.CardNumber}");
                    return $"Something went wrong while Charging the CreditCard {creditCard.CardNumber}";
                }
            }
            else
            {
                ErrorMessages.Add($"{creditCard.CardHolderName}:InSufficient Funds ${creditCard.Balance} available to process the charge ${creditCardActions.Amount}.");
                return $"{creditCard.CardHolderName}:InSufficient Funds ${creditCard.Balance} available to process the charge ${creditCardActions.Amount}.";
            }
        }

        public async Task<List<string>> ExecuteCommands(List<string> inputCommands)
        {
            List<string> outputResults = new List<string>();
            Dictionary<string, string> commandHistory = new Dictionary<string, string>();
            try
            {
                foreach (var command in inputCommands)
                {
                    var commandDetail = command.Split(' ');
                    if (commandDetail[0] == CommandType.Add.ToString())
                    {
                        AddCreditCard addCreditCard = new AddCreditCard
                        {
                            CardHolderName = commandDetail[1],
                            CardNumber = commandDetail[2],
                            CreditLimit = Convert.ToDecimal(commandDetail[3][1..])
                        };
                        var addResult = await AddCreditCardAsync(addCreditCard);

                        //commandHistory.Add(commandDetail[1], command);
                       
                    }
                    else if (commandDetail[0] == CommandType.Charge.ToString())
                    {
                        CreditCardActions creditCardActions = new CreditCardActions { Amount = Convert.ToDecimal(commandDetail[2][1..]), CardHolderName = commandDetail[1], CommandType = CommandType.Charge.ToString() };
                        var chargeResult = await ChargeCreditCardAsync(creditCardActions);
                        //commandHistory.Add(commandDetail[1], command);
                    }
                    else if (commandDetail[0] == CommandType.Credit.ToString())
                    {
                        CreditCardActions creditCardActions = new CreditCardActions { Amount = Convert.ToDecimal(commandDetail[2][1..]), CardHolderName = commandDetail[1], CommandType = CommandType.Credit.ToString() };
                        var chargeResult = await AddCashBackAsync(creditCardActions);
                        //commandHistory.Add(commandDetail[1], command);
                    }
                    else if (commandDetail[0] == CommandType.History.ToString())
                    {
                        if (commandHistory.ContainsKey(commandDetail[1]))
                        {
                            outputResults.AddRange(commandHistory.Values.ToList());
                        }
                    }

                    //Add Command to the History Table
                    var commandDbObject = PrepareCommandDbObject(command, commandDetail[1]);
                    var addCommandResult = await commandService.AddCommand(commandDbObject);

                }

                logger.LogInformation($"All Commands Executed Successfully");
                var resultSet = await creditCardService.ListCreditCardsAsync();

                foreach (var item in resultSet)
                {
                    outputResults.Add($"{item.CardHolderName}:${item.Balance}");
                }
                outputResults.AddRange(ErrorMessages);
            }
            catch (Exception ex)
            {
                throw new ProcessCreditCardException(ex.Message, ex);
            }
            outputResults.Sort();
            return outputResults;
        }

        public async Task<List<CreditCard>> ListCreditCardsAsync()
        {
            try
            {
                var resultSet = await creditCardService.ListCreditCardsAsync();
                return resultSet.ToList();
            }
            catch (Exception ex)
            {
                throw new ProcessCreditCardException(ex.Message, ex);
            }
        }

        public async Task<CreditCard> GetCreditCardAsync(string cardHolderName)
        {
            try
            {
                 var resultSet = await creditCardService.GetCreditCardInfoAsync(cardHolderName);
            return resultSet;
            }
            catch (Exception ex)
            {
                throw new ProcessCreditCardException(ex.Message, ex);
            }
           
        }


        public async Task<List<Command>> ListCommands(string cardHolderName)
        {
            var resultSet = await commandService.ListCommands(cardHolderName);
            return resultSet;
        }

        private Command PrepareCommandDbObject(string command,string cardHolderName)
        {
            return new Command
            {
                CardHolderName = cardHolderName,
                CommandId = Guid.NewGuid(),
                CreditCommand = command
            };
        }
    }
}
