using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProcessCreditCard.Data.Entity;
using ProcessCreditCard.Domain.Biz;
using ProcessCreditCard.Domain.Model;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProcessCreditCard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardController : BaseController
    {
        #region Properties

        private readonly ICreditCardBiz creditCardBiz;

        private readonly ILogger<CreditCardController> logger;

        #endregion

        #region Constructor

        public CreditCardController(ICreditCardBiz creditCardBiz, ILogger<CreditCardController> logger)
        {
            this.creditCardBiz = creditCardBiz;
            this.logger = logger;
        }

        #endregion

        #region ActionMethods


        [HttpPost("Add")]
        public async Task<IActionResult> AddCreditCard([FromBody] List<string> commands)
        {
            logger.LogInformation($"Inside {nameof(AddCreditCard)} Method");

            var results = await creditCardBiz.ExecuteCommands(commands);

            return PackageData(results, HttpStatusCode.OK);
        }


        [HttpPost]
        [SwaggerRequestExample(typeof(AddCreditCard), typeof(AddCreditCardExample))]
        public async Task<IActionResult> AddCreditCard([FromBody] AddCreditCard addCreditCard)
        {
            logger.LogInformation($"Inside {nameof(AddCreditCard)} Method. Add Credit-Card POST API Started");
            var result = await creditCardBiz.AddCreditCardAsync(addCreditCard);
            return PackageData(result, HttpStatusCode.Created);
        }


        [HttpPatch("Charge")]
        public async Task<IActionResult> ChargeCreditCard([FromBody] CreditCardActions creditCardActions)
        {
            logger.LogInformation($"Inside {nameof(ChargeCreditCard)} Method.");
            string result = await creditCardBiz.ChargeCreditCardAsync(creditCardActions);
            return PackageData(result, System.Net.HttpStatusCode.Accepted);
        }

        [HttpPatch("Credit")]
        public async Task<IActionResult> ApplyCashBack([FromBody] CreditCardActions creditCardActions)
        {
            logger.LogInformation($"Inside {nameof(ApplyCashBack)} method.");
            string result = await creditCardBiz.AddCashBackAsync(creditCardActions);
            return PackageData(result, HttpStatusCode.Accepted);
        }

        [HttpGet("List")]
        public async Task<IActionResult> ListAllCards()
        {
            List<Data.Entity.CreditCard> result = await creditCardBiz.ListCreditCardsAsync();
            if (result.Count > 0)
            {
                logger.LogInformation($"Result:- {JsonConvert.SerializeObject(result)}");
                return PackageData(result, HttpStatusCode.OK);
            }

            return new NotFoundObjectResult("No Credit Card Accounts are available in the System");
        }

        [HttpGet("{CardHolderName}")]
        public async Task<IActionResult> GetCreditCardDetail(string CardHolderName)
        {
            CreditCard creditCard = await creditCardBiz.GetCreditCardAsync(CardHolderName);
            if (creditCard != null)
            {
                logger.LogInformation($"Result:- {JsonConvert.SerializeObject(creditCard)}");
                return PackageData(creditCard, HttpStatusCode.OK);
            }

            return new NotFoundObjectResult("No Credit Card Accounts are available in the System");
        }

        [HttpGet("History/{cardholdername}")]
        public async Task<IActionResult> ListCommands(string cardholdername)
        {
            if (string.IsNullOrEmpty(cardholdername))
            {
                var resultSet = await creditCardBiz.ListCommands(cardholdername);
                if (resultSet.Count > 0)
                {
                    return PackageData(resultSet, HttpStatusCode.OK);
                }
                else
                {
                    return new NotFoundObjectResult($"No Commands were executed for {cardholdername}");
                }
            }
            return BadRequest();
        }

        #endregion

    }
}
