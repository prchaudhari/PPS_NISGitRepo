// <copyright file="SQLAzureTenantUserRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{

    #region References

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Security.Cryptography;
    using System.Text;
    using Unity;

    #endregion

    public class SQLTenantUserRepository : ITenantUserRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        #endregion

        #region Constructor

        public SQLTenantUserRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);

        }

        #endregion

        #region Public Methods

        #region Add 

        /// <summary>
        /// This method helps to adds the specified list of users in user entity.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool AddTenantUsers(IList<TenantUser> users, string tenantCode)
        {
            bool result = false;
            try
            {
                //this.connectionString = "metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=192.168.100.7;Initial Catalog=nvidyo;TenantUser ID=sa;Password=Admin@123;multipleactiveresultsets=True;application name=EntityFramework';";

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateTenantUserEmailAndMobileNumber(users, ModelConstant.ADD_OPERATION, tenantCode))
                {
                    throw new DuplicateTenantUserFoundException(tenantCode);
                }

                IList<TenantUserRecord> userRecords = new List<TenantUserRecord>();

                users.ToList().ForEach(user =>
                {
                    userRecords.Add(new TenantUserRecord()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        ContactNumber = user.ContactNumber,
                        EmailAddress = user.EmailAddress,
                        Image = user.Image,
                        IsActive = user.IsActive,
                        IsDeleted = false,
                        IsLocked = false,
                        NoofAttempts = 0,
                        CountryId = user.CountryId,
                        TenantCode = tenantCode
                    });
                });

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {

                    nVidYoEntitiesDataContext.TenantUserRecords.AddRange(userRecords);
                    nVidYoEntitiesDataContext.SaveChanges();
                }

                users.ToList().ForEach(user =>
                {
                    user.Identifier = userRecords.Where(x => x.EmailAddress == user.EmailAddress).Single().Id;
                   // this.MapTenantUserAndRole(user.Roles, user.Identifier, tenantCode);
                });

                result = true;
            }

            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update

        /// <summary>
        /// This method helps to updates the specified list of users in uaer entity.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool UpdateTenantUsers(IList<TenantUser> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateTenantUserEmailAndMobileNumber(users, ModelConstant.UPDATE_OPERATION, tenantCode))
                {
                    throw new DuplicateTenantUserFoundException(tenantCode);
                }


                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", users.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    IList<TenantUserRecord> userRecords = nVidYoEntitiesDataContext.TenantUserRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (userRecords == null || userRecords.Count <= 0 || userRecords.Count() != string.Join(",", userRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new TenantUserNotFoundException(tenantCode);
                    }

                    users.ToList().ForEach(user =>
                    {
                        TenantUserRecord userRecord = nVidYoEntitiesDataContext.TenantUserRecords.FirstOrDefault(data => data.Id == user.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        userRecord.FirstName = user.FirstName;
                        userRecord.LastName = user.LastName;
                        userRecord.ContactNumber = user.ContactNumber;
                        userRecord.EmailAddress = user.EmailAddress;
                        userRecord.Image = user.Image;
                        userRecord.IsActive = user.IsActive;
                        //userRecord.IsLocked = false;
                        userRecord.NoofAttempts = 0;
                        userRecord.TenantCode = tenantCode;
                        userRecord.CountryId = user.CountryId;
                    });

                    nVidYoEntitiesDataContext.SaveChanges();
                }

                users.ToList().ForEach(user =>
                {
                    //this.MapTenantUserAndRole(user.Roles, user.Identifier, tenantCode);
                });

                result = true;
            }

            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Delete

        /// <summary>
        /// This method helps to deletes the specified list of users in user repository. 
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool DeleteTenantUsers(IList<TenantUser> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", users.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append("and IsDeleted.Equals(false)");
                    IList<TenantUserRecord> userRecords = nVidYoEntitiesDataContext.TenantUserRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (userRecords == null || userRecords.Count <= 0)
                    {
                        throw new TenantUserNotFoundException(tenantCode);
                    }

                    userRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                    });

                    nVidYoEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// This method helps to retrieve list of user based on specified search paramters.
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantUser> GetTenantUsers(TenantUserSearchParameter userSearchParameter, string tenantCode)
        {
            IList<TenantUser> users = new List<TenantUser>();
            IList<TenantUserRecord> userRecords = null;
            IList<CountryRecord> countires = new List<CountryRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntities = new NISEntities(this.connectionString))
                {
                    string whereClause = this.WhereClauseGenerator(userSearchParameter, tenantCode);
                    userRecords = new List<TenantUserRecord>();

                    if (userSearchParameter.PagingParameter.PageIndex > 0 && userSearchParameter.PagingParameter.PageSize > 0)
                    {
                        userRecords = nISEntities.TenantUserRecords
                        .OrderBy(userSearchParameter.SortParameter.SortColumn + " " + userSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((userSearchParameter.PagingParameter.PageIndex - 1) * userSearchParameter.PagingParameter.PageSize)
                        .Take(userSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        userRecords = nISEntities.TenantUserRecords
                        .Where(whereClause)
                        .OrderBy(userSearchParameter.SortParameter.SortColumn + " " + userSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }
                    if (userRecords?.Count() > 0)
                    {
                        StringBuilder countryIdQuery = new StringBuilder();
                        countryIdQuery = countryIdQuery.Append("(" + string.Join("or ", userRecords.Select(item => string.Format("Id.Equals({0}) ", item.CountryId))) + ")");
                        countires = nISEntities.CountryRecords.Where(countryIdQuery.ToString()).ToList();
                    }
                }
                IList<TenantUser> tempTenantUsers = new List<TenantUser>();
                userRecords?.ToList().ForEach(userRecord =>
                {
                    string contactnumber = countires.Where(i => i.Id == userRecord.CountryId).FirstOrDefault().DialingCode + "-" + userRecord.ContactNumber;

                    tempTenantUsers.Add(new TenantUser()
                    {
                        Identifier = userRecord.Id,
                        FirstName = userRecord.FirstName,
                        LastName = userRecord.LastName,
                        EmailAddress = userRecord.EmailAddress,
                        ContactNumber = contactnumber,
                        CountryId = (long)userRecord.CountryId,
                        Image = userRecord.Image,
                        IsActive = userRecord.IsActive,
                        IsLocked = userRecord.IsLocked,
                        NoofAttempts = userRecord.NoofAttempts,
                        TenantCode = tenantCode.Equals(ModelConstant.DEFAULT_TENANT_CODE) ? userRecord.TenantCode : tenantCode,
                        //Roles = roles,

                    });

                });
                users = tempTenantUsers;

            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return users;
        }

        /// <summary>
        /// This method helps to retrieve list of user based on specified search paramters.
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantUser> GetTenantUserToAuthentication(TenantUserSearchParameter userSearchParameter, string tenantCode)
        {
            IList<TenantUser> users = null;
            IList<TenantUserRecord> userRecords = null;

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string dateFormat = string.Empty;
                using (NISEntities nISEntities = new NISEntities(this.connectionString))
                {
                    userRecords = nISEntities.TenantUserRecords.Where(item => item.EmailAddress == userSearchParameter.EmailAddress).ToList();
                    dateFormat = nISEntities.TenantConfigurationRecords.Where(item => item.TenantCode == tenantCode).ToList().FirstOrDefault().DateFormat;
                }
                users = new List<TenantUser>();
                userRecords?.ToList().ForEach(userRecord =>
                {
                   // IList<Role> roles = this.GetRoles(userRecord.Id, userSearchParameter.IsRolePrivilegesRequired, userRecord.TenantCode);
                    users.Add(new TenantUser()
                    {
                        Identifier = userRecord.Id,
                        FirstName = userRecord.FirstName,
                        LastName = userRecord.LastName,
                        EmailAddress = userRecord.EmailAddress,
                        ContactNumber = userRecord.ContactNumber,
                        Image = userRecord.Image,
                        IsActive = userRecord.IsActive,
                        IsLocked = userRecord.IsLocked,
                        NoofAttempts = userRecord.NoofAttempts,
                        TenantCode = tenantCode.Equals(ModelConstant.DEFAULT_TENANT_CODE) ? userRecord.TenantCode : tenantCode,
                        //Roles = roles,
                        DateFormat = dateFormat
                    });

                });

            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return users;
        }

        #endregion


        #region Get Count

        /// <summary>
        /// this method helps to get user count
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetTenantUserCount(TenantUserSearchParameter userSearchParameter, string tenantCode)
        {
            int userCount = 0;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(userSearchParameter, tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    userCount = nVidYoEntitiesDataContext.TenantUserRecords
                                                      .Where(whereClause.ToString())
                                                      .Count();
                }
                return userCount;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Activate

        /// <summary>
        /// This method will be used to activate user
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns true is activated user successfully otherwise false</returns>
        public bool ActivateTenantUser(long userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    TenantUserRecord userRecord = nVidYoEntitiesDataContext.TenantUserRecords.FirstOrDefault(x => x.Id == userIdentifier);
                    userRecord.IsActive = true;
                    nVidYoEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This method will be used to deactivate user
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns true is activated user successfully otherwise false</returns>
        public bool DeactivateTenantUser(long userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    TenantUserRecord userRecord = nVidYoEntitiesDataContext.TenantUserRecords.FirstOrDefault(x => x.Id == userIdentifier);
                    userRecord.IsActive = false;
                    nVidYoEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region Check Duplicate TenantUser

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        public bool IsDuplicateTenantUserEmailAndMobileNumber(IList<TenantUser> users, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", users.Select(item => string.Format("EmailAddress.Equals(\"{0}\")",
                  item.EmailAddress))) + "");
                }
                else
                {
                    query.Append("(" + string.Join(" or ", users.Select(item => string.Format("EmailAddress.Equals(\"{0}\") and !Id.Equals({1})",
                      item.EmailAddress, item.Identifier))) + "");
                }
                query.Append(string.Format(" and TenantCode.Equals(\"{0}\")", tenantCode));
                query.Append(" and IsDeleted.Equals(false))");

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<TenantUserRecord> userRecords = nVidYoEntitiesDataContext.TenantUserRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (userRecords.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }
        #endregion

        #region WhereClause Generator

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">TenantUser search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(TenantUserSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            try
            {
                if (!string.IsNullOrWhiteSpace(searchParameter.TenantCode))
                {
                    tenantCode = searchParameter.TenantCode;
                }

                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(searchParameter.TenantUserIdentifierToSkip))
                {
                    //queryString.Append("(" + string.Join("and ", searchParameter.TenantUserIdentifierToSkip.ToString().Split(',').Select(item => string.Format(" !Id.Equals({0}) ", Convert.ToInt64(item)))) + ") and ");
                    queryString.Append("(" + string.Join("and ", searchParameter.TenantUserIdentifierToSkip.ToString().Split(',').Select(item => string.Format("!Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(searchParameter.FirstName))
                {
                    // queryString.Append(queryString.Length > 0 ? " and " : string.Empty);
                    if (searchParameter.SearchMode == SearchMode.Equals)
                    {
                        // queryString.Append(string.Format("FirstName.Equals(\"{0}\") or LastName.Equals(\"{0}\") and ", searchParameter.FirstName, searchParameter.LastName));
                        queryString.Append("(" + string.Join("or ", searchParameter.FirstName.ToString().Split(',').Select(item => string.Format("(FirstName+\" \"+LastName).Contains(\"{0}\")", item))) + ") and ");

                    }
                    else
                    {
                        //queryString.Append(string.Format("FirstName.Contains(\"{0}\") or LastName.Equals(\"{0}\") and ", searchParameter.FirstName, searchParameter.LastName));
                        queryString.Append("(" + string.Join("or ", searchParameter.FirstName.ToString().Split(',').Select(item => string.Format("(FirstName+\" \"+LastName).Contains(\"{0}\")", item))) + ") and ");


                    }
                }

                if (validationEngine.IsValidText(searchParameter.EmailAddress))
                {
                    //queryString.Append(queryString.Length > 0 ? " and " : string.Empty);
                    if (searchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("EmailAddress.Equals(\"{0}\") and ", searchParameter.EmailAddress));
                    }
                    else
                    {
                        queryString.Append(string.Format("EmailAddress.Contains(\"{0}\") and ", searchParameter.EmailAddress));
                    }
                }

                if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
                {
                    queryString.Append(string.Format(" TenantCode.Equals(\"{0}\") and ", tenantCode));
                }

                if (searchParameter.IsActive != null)
                {
                    queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.IsActive));
                }
                if (searchParameter.ActivationStatus != null)
                {
                    queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.ActivationStatus));
                }
                if (searchParameter.LockStatus != null)
                {
                    queryString.Append(string.Format("IsLocked.Equals({0}) and ", searchParameter.LockStatus));
                }
                queryString.Append("IsDeleted.Equals(false)");
            }

            catch (Exception exception)
            {
                throw exception;
            }

            return queryString.ToString();
        }

        #endregion

        #region Get Connection String

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// Decrypts the specified encrypted text.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string encryptedText)
        {

            string PasswordHash = "p@$$word";
            string SaltKey = "S@LT&KEY";
            string VIKey = "@1B2c3D4e5F6g7H8i9F";

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

        #endregion

    }
}
