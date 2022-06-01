// <copyright file="ValidationEngine.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace NedBankValidationEngine
{
    #region References


    #endregion

    /// <summary>
    /// This class represents methods to validate data.
    /// </summary>
    public class NedBankValidationEngine : INedBankValidationEngine
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
            if (text.IndexOfAny(NedBankValidationEngineConstant.restrictedCharacters) >= 0)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
