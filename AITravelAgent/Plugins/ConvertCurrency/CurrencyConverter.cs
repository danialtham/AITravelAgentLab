using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelAgent.Plugins.ConvertCurrency
{
    public class CurrencyConverter
    {
        public static string ConvertAmount(string amount, string baseCurrencyCode, string targetCurrencyCode)
        {
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
        }
    }
}
