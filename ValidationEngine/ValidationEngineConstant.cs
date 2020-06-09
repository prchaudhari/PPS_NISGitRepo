// <copyright file="ValidationEngineConstant.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2016 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    /// <summary>
    /// This class represents constant values.
    /// </summary>
    public class ValidationEngineConstant
    {
        #region Public Members

        /// <summary>
        /// Indicates pattern to validate email address.
        /// </summary>
        public const string MatchEmailPattern =
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                        [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                        [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        /// <summary>
        /// Restricted characters
        /// </summary>
        public readonly static char[] restrictedCharacters = new char[] { '<', '>', ',', ';', '"' };

        #endregion
    }
}
