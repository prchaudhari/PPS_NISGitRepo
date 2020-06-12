// <copyright file="CryptoManager.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2016 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    public class CryptoManager : ICryptoManager
    {
        #region private Members

        /// <summary>
        /// The password in hash code
        /// Readonly prevents fields from being changed. 
        /// Readonly fields can be initialized at runtime, unlike constant values. Attempts to change them later are disallowed.
        /// </summary>
        private static readonly string PasswordHash = "p@$$word";

        /// <summary>
        /// The salt key. 
        /// Readonly prevents fields from being changed. 
        /// Readonly fields can be initialized at runtime, unlike constant values. Attempts to change them later are disallowed.
        /// </summary>
        private static readonly string SaltKey = "S@LT&KEY";

        /// <summary>
        /// The VI key
        /// Readonly prevents fields from being changed. 
        /// Readonly fields can be initialized at runtime, unlike constant values. Attempts to change them later are disallowed.
        /// </summary>
        private static readonly string VIKey = "@1B2c3D4e5F6g7H8i9F";

        #endregion

        #region Public Methods

        /// <summary>
        /// Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string plainText)
        {
            //// Converts the plain text to byte array of plain text
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            //// Returns pseudo random key using byte array of plain text and salt key
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);

            //// RijndaelManaged class Accesses the managed version of the Rijndael algorithm using CBC(Cipher block chaining) mode and padding
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };

            //// The transformation used for encryption
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            byte[] cipherTextBytes;
            using (var memoryStream = new MemoryStream())
            {
                //// initialises new instance of crypto stream class with target data stream,the transformation to use and mode of stream
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    //// Writes a sequence of bytes to the current CryptoStream and advances
                    //// the current position within the stream by the number of bytes written.Overrides Stream.Write(Byte[], Int32, Int32)
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                    //// Flush final block
                    cryptoStream.FlushFinalBlock();

                    //// Converts memory stream data to byte array
                    cipherTextBytes = memoryStream.ToArray();

                    //// Closing crypto stream
                    cryptoStream.Close();
                }

                //// Closing memory stream
                memoryStream.Close();
            }

            //// Convert the cipher text byte array to string
            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>
        /// Decrypts the specified encrypted text.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText.Replace(' ', '+'));

            //// Returns pseudo random key using byte array of plain text and salt key
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);

            //// RijndaelManaged class Accesses the managed version of the Rijndael algorithm using CBC(Cipher block chaining) mode and padding
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };

            //// The transformation used for decryption
            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            //// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
            //// Overrides Stream.Read(Byte[], Int32, Int32)
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            //// Close memory stream
            memoryStream.Close();

            //// Closing crypto stream
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        /// <summary>
        /// Method to encrypt the password
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>The encrypt string</returns>
        public string EncryptPassword(string password)
        {
            byte[] encodedBytes;

            using (var md5 = new MD5CryptoServiceProvider())
            {
                var originalBytes = Encoding.Default.GetBytes(password);
                encodedBytes = md5.ComputeHash(originalBytes);
            }
            return Convert.ToBase64String(encodedBytes);
        }

        #endregion
    }
}
