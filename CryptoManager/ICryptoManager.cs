// <copyright file="ICryptoManager.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2015 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    /// <summary>
    /// Represents interface which have methods encrypt and decrypt
    /// </summary>
    public interface ICryptoManager
    {
        #region Public Methods

        /// <summary>
        /// This method encrypts plain text data
        /// </summary>
        /// <param name="plainText">The plain text</param>
        /// <returns>String of encrypted text</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// This method Decrypts encrypted text data
        /// </summary>
        /// <param name="encryptedText">The encrypted text</param>
        /// <returns>String of Decrypted text</returns>
        string Decrypt(string encryptedText);

        /// <summary>
        /// This to encrypt password using MD5
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>String of encrypted text</returns>
        string EncryptPassword(string password);

        #endregion
    }
}
