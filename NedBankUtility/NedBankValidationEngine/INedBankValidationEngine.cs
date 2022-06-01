// <copyright file="INedBankValidationEngine.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//--

namespace NedBankValidationEngine
{
    #region Reference 
    #endregion

    /// <summary>
    /// This interface represents reference of methods.
    /// </summary>
    public interface INedBankValidationEngine
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

        #endregion
    }
}
