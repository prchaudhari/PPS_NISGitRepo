using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public static class CommonUtility
    {

        public static string concatRWithDouble(string value)
        {
            if (value.Contains("R"))
                return value;

            decimal amount = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture); // Parse the string to a decimal

            string formattedAmount = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-ZA"));
            return formattedAmount.Replace(",", ".");
        }
        public static string GetMonthRange(int month)
        {
            switch (month)
            {
                case 1: return "1 Jan - 31 Jan";
                case 2: return "1 Feb - 28 Feb";
                case 3: return "1 Mar - 31 Mar";
                case 4: return "1 Apr - 30 Apr";
                case 5: return "1 May - 31 May";
                case 6: return "1 Jun - 30 Jun";
                case 7: return "1 Jul - 31 Jul";
                case 8: return "1 Aug - 31 Aug";
                case 9: return "1 Sep - 30 Sep";
                case 10: return "1 Oct - 31 Oct";
                case 11: return "1 Nov - 30 Nov";
                case 12: return "1 Dec - 31 Dec";
                default: return "Invalid Month";
            }
        }

    }
}
