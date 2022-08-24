using MeghdadIT.Interfaces;
using MeghdadIT.Models.ApiModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MeghdadIT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConverter _currencyConvertor;
        public CurrencyController(ICurrencyConverter currencyConvertor)
        {
            this._currencyConvertor = currencyConvertor;
        }
        [HttpPut("UpdateConfiguration")]
        public IActionResult UpdateConfiguration(IEnumerable<Api_SetCurrencyConfigModel> model)
        {
            try
            {
                this._currencyConvertor.UpdateConfiguration(model.Select(x => Tuple.Create(x.FromCurrency, x.ToCurrency, x.Rate)).ToList());
                return Ok(new { Status = HttpStatusCode.OK, Message = "Currency Configuration updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Status = HttpStatusCode.InternalServerError, Message = "Updating Currency Configuration failed.", ExMessage = ex.Message });
            }
        }
        [HttpDelete("ClearConfiguration")]
        public IActionResult ClearConfiguration()
        {
            try
            {
                this._currencyConvertor.ClearConfiguration();
                return Ok(new { Status = HttpStatusCode.OK, Message = "Currency Configuration removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Status = HttpStatusCode.InternalServerError, Message = "Removing Currency Configuration failed.", ExMessage = ex.Message });
            }
        }
        [HttpPost("Convert")]
        public IActionResult Convert([FromQuery] Api_ConvertRequestModel model)
        {
            try
            {
                var finalAmount = this._currencyConvertor.Convert(model.FromCurrency, model.ToCurrency, model.Amount);
                IActionResult result = finalAmount switch
                {
                    -400 => BadRequest(new { Status = HttpStatusCode.BadRequest, Message = "Entered data is not valid" }),
                    -404 => NotFound(new { Status = HttpStatusCode.NotFound, Message = "Configuration not found" }),
                    _ => Ok(new { Status = HttpStatusCode.OK, Message = $"Entered Amount : {model.Amount} , From Currency : {model.FromCurrency.ToUpper()} , To Currency : {model.ToCurrency.ToUpper()} , Final Amount : {finalAmount}", FinalAmount = finalAmount }),
                };
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Status = HttpStatusCode.InternalServerError, Message = "Conversion failed.", ExMessage = ex.Message });
            }
            
        }
    }
}
