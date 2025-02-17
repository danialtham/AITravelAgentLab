using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelAgent.Plugins.ConvertCurrency
{
    public class CurrencyConverter
    {
        [KernelFunction("ConvertAmount")]
        [Description("Converts an amount from one currency to another")]
        public static string ConvertAmount(string amount, string baseCurrencyCode, string targetCurrencyCode)
        {
            var currencyDictionary = Currency.Currencies;

            Currency targetCurrency = currencyDictionary[targetCurrencyCode];
            Currency baseCurrency = currencyDictionary[baseCurrencyCode];

            if (targetCurrency == null)
            {
                return targetCurrencyCode + " was not found";
            }
            else if (baseCurrency == null)
            {
                return baseCurrencyCode + " was not found";
            }
            else
            {
                double amountInUSD = double.Parse(amount) * baseCurrency.USDPerUnit;
                double result = amountInUSD * targetCurrency.UnitsPerUSD;
                return result + targetCurrencyCode;
            }

        }
    }
}
