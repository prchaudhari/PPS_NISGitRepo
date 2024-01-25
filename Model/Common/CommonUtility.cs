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
            // Initialize the result variable
            string result;

            // Declare a variable to store the parsed double value
            double valueDbl;

            // Try to parse the input value to a double
            bool success = double.TryParse(value, out valueDbl);

            // Check if parsing was successful
            if (success)
            {
                // Check if the parsed double value is negative
                if (valueDbl < 0)
                {
                    result = "-R" + (-valueDbl).ToString("F2");
                }
                // Check if the parsed double value is zero
                else if (valueDbl == 0)
                {
                    result = "R" + "0.00";
                }
                // If the parsed double value is positive
                else
                {
                    result = "R" + valueDbl.ToString("F2");
                }

                // Return the result
                return result;
            }
            else
            {
                // If parsing fails, print an error message and return null
                Console.WriteLine($"Attempted conversion of '{value ?? "<null>"}' failed.");
                return result = null;
            }
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
