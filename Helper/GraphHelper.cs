using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeghdadIT.Helper
{
    public class GraphHelper
    {
        private Dictionary<string, List<string>> _currenyGraph;
        private List<Tuple<string, string, double>> _currencyRates;
        public GraphHelper(IEnumerable<Tuple<string, string, double>> rates)
        {
            // add configuration to private property of class
            this._currencyRates = new List<Tuple<string, string, double>>();
            this._currencyRates.AddRange(rates);


            // create a graph of configurations on counstructing class
            this._currenyGraph = new Dictionary<string, List<string>>();
            foreach (var rate in rates)
            {
                if (!this._currenyGraph.ContainsKey(rate.Item1))
                {
                    this._currenyGraph[rate.Item1] = new List<string>();
                }
                if (!this._currenyGraph.ContainsKey(rate.Item2))
                {
                    this._currenyGraph[rate.Item2] = new List<string>();
                }
                this._currenyGraph[rate.Item1].Add(rate.Item2);
                this._currenyGraph[rate.Item2].Add(rate.Item1);
            }
        }
        private double GetFlatRate(string fromCurrency, string toCurrency)
        {
            // flat rate (from -> to) retuns rate, (to -> from) retuns 1/rate
            if (fromCurrency == toCurrency)
            {
                return 1;
            }
            if (this._currencyRates.Any(x => x.Item1 == fromCurrency && x.Item2 == toCurrency))
            {
                return this._currencyRates.FirstOrDefault(x => x.Item1 == fromCurrency && x.Item2 == toCurrency).Item3;
            }
            return 1 / (this._currencyRates.FirstOrDefault(x => x.Item1 == toCurrency && x.Item2 == fromCurrency)).Item3;
        }
        public double RateFinder(string fromCurrency, string toCurrency)
        {
            //if fromcurrency and tocurrency are straight relation
            if (this._currenyGraph[fromCurrency].Contains(toCurrency))
            {
                return this.GetFlatRate(fromCurrency, toCurrency);
            }
            //find path
            else
            {
                foreach (var currency in this._currenyGraph[fromCurrency])
                {

                    //loop function to find item 2 of index is item 1 of next index
                    double rate = RateFinder(currency, toCurrency);
                    if (rate != 0)
                    {
                        return rate * this.GetFlatRate(fromCurrency, currency);
                    }
                       
                }
            }
            return 0;
        }
    }
}
