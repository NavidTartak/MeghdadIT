using MeghdadIT.Helper;
using MeghdadIT.Interfaces;
using MeghdadIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeghdadIT.Implementations
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private List<Tuple<string, string, double>> _ConversionRates { get; set; } = new();
        public void ClearConfiguration()
        {
            //Clear All Configuration

            this._ConversionRates = new List<Tuple<string, string, double>>();
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            //Checknig
            if (this._ConversionRates == null || !this._ConversionRates.Any())
            {
                return -404;
            }
            if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrWhiteSpace(fromCurrency) || fromCurrency.Length != 3 || string.IsNullOrEmpty(toCurrency) || string.IsNullOrWhiteSpace(toCurrency) || toCurrency.Length != 3 || amount < 0)
            {
                return -400;
            }
            if (amount == 0)
            {
                return 0;
            }
            //Convert config to Graph
            GraphHelper graphHelper = new(this._ConversionRates);
            var rate = graphHelper.RateFinder(fromCurrency.ToUpper(), toCurrency.ToUpper());
            if (rate == 0)
            {
                return -404;
            }
            return amount * rate;

        }

        public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
        {
            //Check entered List 
            conversionRates = conversionRates.Where(config =>
            !string.IsNullOrEmpty(config.Item1) && config.Item1.Replace(" ", "").Length == 3 &&
            !string.IsNullOrEmpty(config.Item2) && config.Item2.Replace(" ", "").Length == 3 &&
             config.Item3 > 0).Select(config => Tuple.Create(config.Item1.ToUpper(), config.Item2.ToUpper(), config.Item3)).Where(x =>
             x.Item1 != x.Item2).GroupBy(x => new { x.Item1, x.Item2 }).Select(x => x.First()).ToList();

            //inserting entered list into private propery
            if (this._ConversionRates == null || !this._ConversionRates.Any())
            {
                this._ConversionRates = new List<Tuple<string, string, double>>();
                this._ConversionRates.AddRange(conversionRates);
                return;
            }
            //(Alter)! inserting into or updating entered list into private propery
            foreach (var item in conversionRates)
            {
                var config = this._ConversionRates.FirstOrDefault(x => x.Item1 == item.Item1 && x.Item2 == item.Item2);
                if (config == null)
                {
                    this._ConversionRates.Add(item);
                }
                else
                {
                    this._ConversionRates.Remove(config);
                    this._ConversionRates.Add(item);
                }
            }
            return;

        }
    }
}
