// <copyright file="ValidationEngine.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Newtonsoft.Json;
    #region References

    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// This class represents methods to validate data.
    /// </summary>
    public class ValidationEngine : IValidationEngine
    {
        #region Public Methods

        /// <summary>
        /// This method validate string which is not null, not emtpy and not only while spaces.
        /// </summary>
        /// <param name="text">Text which want to validate.</param>
        /// <returns>If string is not empty , not null and not white spaces then it will return true otherwise flase.</returns>
        public bool IsValidText(string text, bool isCheckValidCharacters = false)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (isCheckValidCharacters)
                {
                    return IsValidCharacters(text);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// This method validates string which does not contain any special characters.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidCharacters(string text)
        {
            if (text.IndexOfAny(ValidationEngineConstant.restrictedCharacters) >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method validates phone number.
        /// </summary>
        /// <param name="number">Phonenumber which want to validate.</param>
        /// <returns>If phone number has less than 20 digit, it will return true otherwise false.</returns>
        public bool IsValidPhonenumber(string number)
        {
            if (number.Trim().Length <= 20)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method validates Guid value.
        /// </summary>
        /// <param name="guid">Guid value which want to validate.</param>
        /// <returns>If guid has not default value then it will return true otherwise false.</returns>
        public bool IsValidGuid(Guid guid)
        {
            if (guid != default(Guid) && guid != Guid.Empty)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public bool IsValidEmail(string email)
        {
            if (email != null)
            {
                return Regex.IsMatch(email, ValidationEngineConstant.MatchEmailPattern);
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is valid date] [the specified parameter value].
        /// </summary>
        /// <param name="date">The parameter value.</param>
        /// <returns>
        /// True, if parameter value is valid date, false otherwise.
        /// </returns>
        public bool IsValidDate(DateTime date)
        {
            if (date > Convert.ToDateTime("1753-01-01") && date > DateTime.MinValue)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method helps to check given string is day of week.
        /// </summary>
        /// <param name="day">
        /// The name is day of week.
        /// </param>
        /// <returns>
        /// It return true or false.
        /// </returns>
        public bool IsValidDay(string day)
        {
            DayOfWeek dayOfWeek;
            if (Enum.TryParse<DayOfWeek>(day, out dayOfWeek))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method helps to check given string is month name.
        /// </summary>
        /// <param name="monthName">
        /// The name is name of month.
        /// </param>
        /// <returns>
        /// It return true or false.
        /// </returns>
        public bool IsValidMonth(string monthName)
        {
            if (DateTimeFormatInfo.CurrentInfo.MonthNames.Any(item => item == monthName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method helps to check given string is month name.
        /// </summary>
        /// <param name="day">
        /// The name is name of month.
        /// </param>
        /// <returns>
        /// It return true or false.
        /// </returns>
        public bool IsValidTime(string time)
        {
            if (time.Split(':').Length == 2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method use to validate long value
        /// </summary>
        /// <param name="longValue">
        /// Value of long
        /// </param>
        /// <returns>
        /// Returna true if value is greater than or equals to 0
        /// </returns>
        public bool IsValidLong(long longValue, bool isZeroAllow = true)
        {
            if (isZeroAllow && longValue >= 0)
            {
                return true;
            }

            if (longValue > 0)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This method use to validate json string value
        /// </summary>
        /// <param name="strInput">
        /// Value of strInput
        /// </param>
        /// <returns>
        /// Returna true if value is correct json string or false
        /// </returns>
        public bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(strInput);
                    return true;
                }
                catch // not valid
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
