// <copyright file="IValidationEngine.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//--

namespace nIS
{
    #region Reference 
    using System;
    #endregion

    /// <summary>
    /// This interface represents reference of methods.
    /// </summary>
    public interface IValidationEngine
    {
        #region Public Methods

        /// <summary>
        /// This method reference will validate string which is not null, not emtpy and which has not only while spaces.
        /// </summary>
        /// <param name="text">
        /// Text which want to validate.
        /// </param>
        /// <param name="isCheckValidCharacters">Set true if want to validate for restricted characters also.</param>
        /// <returns>
        /// If string is not empty , not null and not white spaces then it will return true otherwise flase.
        /// </returns>
        bool IsValidText(string text, bool isCheckValidCharacters = false);

        /// <summary>
        /// This method reference will validate string which will not contain the restricted characters.
        /// </summary>
        /// <param name="text">
        /// Text whcih wants to validate.
        /// </param>
        /// <returns>
        /// If the string does not contain any restricted character, then it will return true otherwise false.
        /// </returns>
        bool IsValidCharacters(string text);

        /// <summary>
        /// This method reference will validate phone number.
        /// </summary>
        /// <param name="number">
        /// Phonenumber which want to validate.
        /// </param>
        /// <returns>
        /// If phone number has 10 digit, it will return true otherwise false.
        /// </returns>
        bool IsValidPhonenumber(string number);

        /// <summary>
        /// This method will validate guid value.
        /// </summary>
        /// <param name="guid">Guid value which want to validate.</param>
        /// <returns>If guid has not default value then it will return true otherwise false.</returns>
        bool IsValidGuid(Guid guid);

        /// <summary>
        /// This method reference will check whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        bool IsValidEmail(string email);

        /// <summary>
        /// Determines whether [is valid date] [the specified parameter value].
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>
        /// True, if parameter value is valid date, false otherwise.
        /// </returns>
        bool IsValidDate(DateTime parameterValue);

        /// <summary>
        /// Determines whether [is valid day of week] [the specified parameter value].
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>
        /// True, if parameter value is valid day of week, false otherwise.
        /// </returns>
        bool IsValidDay(string day);

        /// <summary>
        /// Determines whether [is valid month name] [the specified parameter value].
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>
        /// True, if parameter value is valid month name, false otherwise.
        /// </returns>
        bool IsValidMonth(string monthName);

        /// <summary>
        /// Determines whether [is valid time] [the specified parameter value].
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>
        /// True, if parameter value is valid time, false otherwise.
        /// </returns>
        bool IsValidTime(string monthName);

        /// <summary>
        /// This method use to validate long value
        /// </summary>
        /// <param name="longValue">
        /// Value of long
        /// </param>
        /// <returns>
        /// Returna true if value is greater than or equals to 0
        /// </returns>
        bool IsValidLong(long longValue, bool isZeroCheck = false);
        #endregion
    }
}
