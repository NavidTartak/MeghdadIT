using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeghdadIT.Models.ApiModels
{
    public class Api_SetCurrencyConfigModel
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double Rate { get; set; }
    }
}
