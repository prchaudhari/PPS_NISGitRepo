// <copyright file="SQLAzureUserRepository.cs" company="Websym Solutions Pvt Ltd">
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

    public class SQLUserRepository : IUserRepository
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

        public SQLUserRepository(IUnityContainer unityContainer)
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
        public bool AddUsers(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {
                //this.connectionString = "metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=192.168.100.7;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123;multipleactiveresultsets=True;application name=EntityFramework';";

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateUserEmailAndMobileNumber(users, ModelConstant.ADD_OPERATION, tenantCode))
                {
                    throw new DuplicateUserFoundException(tenantCode);
                }

                IList<UserRecord> userRecords = new List<UserRecord>();

                users.ToList().ForEach(user =>
                {
                    userRecords.Add(new UserRecord()
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
                        TenantCode = tenantCode
                    });
                });

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {

                    nVidYoEntitiesDataContext.UserRecords.AddRange(userRecords);
                    nVidYoEntitiesDataContext.SaveChanges();
                }

                users.ToList().ForEach(user =>
                {
                    user.Identifier = userRecords.Where(x => x.EmailAddress == user.EmailAddress).Single().Id;
                    this.MapUserAndRole(user.Roles, user.Identifier, tenantCode);
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
        public bool UpdateUsers(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateUserEmailAndMobileNumber(users, ModelConstant.UPDATE_OPERATION, tenantCode))
                {
                    throw new DuplicateUserFoundException(tenantCode);
                }

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", users.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    IList<UserRecord> userRecords = nVidYoEntitiesDataContext.UserRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (userRecords == null || userRecords.Count <= 0 || userRecords.Count() != string.Join(",", userRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new UserNotFoundException(tenantCode);
                    }

                    users.ToList().ForEach(user =>
                    {
                        UserRecord userRecord = nVidYoEntitiesDataContext.UserRecords.FirstOrDefault(data => data.Id == user.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        userRecord.FirstName = user.FirstName;
                        userRecord.LastName = user.LastName;
                        userRecord.ContactNumber = user.ContactNumber;
                        userRecord.EmailAddress = user.EmailAddress;
                        userRecord.Image = user.Image;
                        userRecord.IsActive = user.IsActive;
                        userRecord.IsLocked = false;
                        userRecord.NoofAttempts = 0;
                        userRecord.TenantCode = tenantCode;
                    });

                    nVidYoEntitiesDataContext.SaveChanges();
                }

                users.ToList().ForEach(user =>
                {
                    this.MapUserAndRole(user.Roles, user.Identifier, tenantCode);
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
        public bool DeleteUsers(IList<User> users, string tenantCode)
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
                    IList<UserRecord> userRecords = nVidYoEntitiesDataContext.UserRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (userRecords == null || userRecords.Count <= 0)
                    {
                        throw new UserNotFoundException(tenantCode);
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
        public IList<User> GetUsers(UserSearchParameter userSearchParameter, string tenantCode)
        {
            IList<User> users = null;
            IList<UserRecord> userRecords = null;

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntities = new NISEntities(this.connectionString))
                {
                    string whereClause = this.WhereClauseGenerator(userSearchParameter, tenantCode);
                    userRecords = new List<UserRecord>();
                    if (userSearchParameter.PagingParameter.PageIndex > 0 && userSearchParameter.PagingParameter.PageSize > 0)
                    {
                        userRecords = nISEntities.UserRecords
                        .OrderBy(userSearchParameter.SortParameter.SortColumn + " " + userSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((userSearchParameter.PagingParameter.PageIndex - 1) * userSearchParameter.PagingParameter.PageSize)
                        .Take(userSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        userRecords = nISEntities.UserRecords
                        .Where(whereClause)
                        .OrderBy(userSearchParameter.SortParameter.SortColumn + " " + userSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }
                }
                users = new List<User>();
                userRecords?.ToList().ForEach(userRecord =>
                {
                    //Country country = null;

                    ///Get roles
                    IList<Role> roles = this.GetRoles(userRecord.Id, userSearchParameter.IsRolePrivilegesRequired, userRecord.TenantCode);

                    //Get country
                    //country = this.GetCountry(userRecord.CountryId, tenantCode);
                    users.Add(new User()
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
                        Roles = roles
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

        #region User Login 

        /// <summary>
        /// This method will add users initial credentials.
        /// </summary>
        /// <param name="userLoginDetails">
        /// User login object.
        /// </param>
        public void AddUsersCredential(IList<UserLogin> userLoginDetails, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                IList<UserLoginRecord> userLoginRecords = userLoginDetails
                    .Select(userloginItem => new UserLoginRecord()
                    {
                        UserIdentifier = userloginItem.UserIdentifier,
                        Password = userloginItem.UserPassword,
                        LastModifiedOn = DateTime.UtcNow
                    })
                    .ToList();

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nVidYoEntitiesDataContext.UserLoginRecords.AddRange(userLoginRecords);
                    nVidYoEntitiesDataContext.SaveChanges();
                };

            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method helps to get user login detail using user identifier.
        /// </summary>
        /// <param name="userIdentifier">
        /// User identifier
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// Returns UserLogin object.
        /// </returns>
        public UserLogin GetUserAuthenticationDetail(string userIdentifier, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                UserLogin userLoginDetail = default(UserLogin);
                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    userLoginDetail = nVidYoEntitiesDataContext.UserLoginRecords
                               .Where(userItem => userItem.UserIdentifier == userIdentifier)
                               .Select(userloginItem => new UserLogin()
                               {
                                   UserIdentifier = userloginItem.UserIdentifier,
                                   UserEncryptedPassword = userloginItem.Password
                               })
                               .FirstOrDefault();

                    if (userLoginDetail == null)
                    {
                        throw new UserNotFoundException(tenantCode);
                    }
                }; // end database connection

                return userLoginDetail;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Update no of attempts of user

        /// <summary>
        /// This is responsible for update no of attaempts of user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool UpdateUsersNoOfAttempts(string userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append(string.Format("Id.Equals({0}) ", userIdentifier));
                    IList<UserRecord> userRecords = nVidYoEntitiesDataContext.UserRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (userRecords == null || userRecords.Count <= 0 || userRecords.Count() != string.Join(",", userRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new UserNotFoundException(tenantCode);
                    }

                    UserRecord userRecord = nVidYoEntitiesDataContext.UserRecords.FirstOrDefault(data => data.Id.ToString() == userIdentifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                    if (userRecord != null)
                    {
                        int noofattempts = userRecord.NoofAttempts;
                        noofattempts = noofattempts + 1;
                        userRecord.NoofAttempts = noofattempts;
                        nVidYoEntitiesDataContext.SaveChanges();
                    }

                }
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

        #region Update Locked User

        /// <summary>
        ///  This is responsible for update locked status of user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool LockUser(long userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    UserRecord userRecord = nVidYoEntitiesDataContext.UserRecords.FirstOrDefault(x => x.Id == userIdentifier);

                    if (userRecord == null)
                    {
                        throw new UserNotFoundException(tenantCode);
                    }

                    userRecord.IsLocked = true;
                    nVidYoEntitiesDataContext.SaveChanges();
                }
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

        #region Unlock Users

        /// <summary>
        /// This method will be used to unlock user
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns true is activated user successfully otherwise false</returns>
        public bool UnlockUser(long userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    UserRecord userRecord = nVidYoEntitiesDataContext.UserRecords.FirstOrDefault(x => x.Id == userIdentifier);
                    if (userRecord != null)
                    {
                        userRecord.IsLocked = false;
                        userRecord.NoofAttempts = 0;
                        nVidYoEntitiesDataContext.SaveChanges();
                    }
                    else
                    {
                        throw new UserNotFoundException(tenantCode);
                    }
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

        /// <summary>
        /// This method will update user's password.
        /// </summary>
        /// <param name="user">
        /// User object.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code.
        /// </param>
        /// <returns>
        /// If it will update password successfully then it will return true.
        /// If any error will occured then it will throw an exception.
        /// </returns>
        public bool ChangePassword(UserLogin userLoginDetail, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<UserLoginRecord> addedUserLoginRecord = nVidYoEntitiesDataContext.UserLoginRecords
                        .Where(u => u.UserIdentifier == userLoginDetail.UserIdentifier)
                        .ToList();

                    if (addedUserLoginRecord == null)
                    {
                        throw new UserNotFoundException(tenantCode);
                    }
                    //validate previous and and old password by usn decrypt
                    string userPreviousDecryptPassword = this.Decrypt(addedUserLoginRecord[0].Password);
                    string userNewDecryptPassword = this.Decrypt(userLoginDetail.UserEncryptedPassword.ToString());
                    if (userPreviousDecryptPassword == userNewDecryptPassword)
                    {
                        throw new AlreadyUsedPasswordException(tenantCode);
                    }

                    addedUserLoginRecord[0].Password = userLoginDetail.UserEncryptedPassword.ToString();
                    addedUserLoginRecord[0].LastModifiedOn = DateTime.Now;

                    nVidYoEntitiesDataContext.SaveChanges();
                }; // end db connection                   

                result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will validate user login.
        /// </summary>
        /// <param name="userIdentifier">
        /// User identifier
        /// </param>
        /// <param name="password">
        /// User password.
        /// </param>
        /// <returns>
        /// If password will be correct then it will return true otherwise false.
        /// </returns>
        public bool IsAuthenticatedUser(string userIdentifier, string encryptedPassword, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    UserLoginRecord userLoginRecord = nVidYoEntitiesDataContext.UserLoginRecords
                            .Where(ul => ul.UserIdentifier == userIdentifier && ul.Password == encryptedPassword)
                            .FirstOrDefault();

                    if (userLoginRecord != null)
                    {
                        return true;
                    }

                };

                return false;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Count

        /// <summary>
        /// this method helps to get user count
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetUserCount(UserSearchParameter userSearchParameter, string tenantCode)
        {
            int userCount = 0;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(userSearchParameter, tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    userCount = nVidYoEntitiesDataContext.UserRecords
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
        public bool ActivateUser(long userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    UserRecord userRecord = nVidYoEntitiesDataContext.UserRecords.FirstOrDefault(x => x.Id == userIdentifier);
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
        public bool DeactivateUser(long userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    UserRecord userRecord = nVidYoEntitiesDataContext.UserRecords.FirstOrDefault(x => x.Id == userIdentifier);
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

        #region Password history validation

        /// <summary>
        /// This is responsible for password history validation
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="newPassword"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool IsPasswordHistoryValidation(string userIdentifier, string newPassword, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    UserCredentialHistoryRecord userLoginRecord = nVidYoEntitiesDataContext.UserCredentialHistoryRecords
                            .Where(ul => ul.UserIdentifier == userIdentifier && ul.Password == newPassword && ul.IsSystemGenerated == false && ul.TenantCode == tenantCode)
                            .FirstOrDefault();

                    if (userLoginRecord == null)
                    {
                        return true;
                    }

                };

                return false;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Add User Crendential History 

        /// <summary>
        /// This method will add users initial credentials.
        /// </summary>
        /// <param name="userLoginDetails">
        /// User login object.
        /// </param>
        public void AddUsersCredentialHistory(IList<UserLogin> userLoginDetails, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                IList<UserCredentialHistoryRecord> userLoginRecords = userLoginDetails
                    .Select(userloginItem => new UserCredentialHistoryRecord()
                    {
                        UserIdentifier = userloginItem.UserIdentifier,
                        Password = userloginItem.UserEncryptedPassword,
                        IsSystemGenerated = userloginItem.IsSystemGenerated,
                        CreatedAt = DateTime.UtcNow,
                        TenantCode = tenantCode
                    })
                    .ToList();

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nVidYoEntitiesDataContext.UserCredentialHistoryRecords.AddRange(userLoginRecords);
                    nVidYoEntitiesDataContext.SaveChanges();
                };

            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Add User activity History 

        /// <summary>
        /// This method will add users login activity
        /// </summary>
        /// <param name="userLoginDetails">
        /// User login object.
        /// </param>
        public bool AddUserLogInActivityHistory(IList<UserLoginActivityHistory> userLoginDetails, string tenantCode)
        {
            try
            {
                bool result = false;
                this.SetAndValidateConnectionString(tenantCode);

                IList<UserLoginActivityHistoryRecord> userLoginRecords = userLoginDetails
                    .Select(userloginItem => new UserLoginActivityHistoryRecord()
                    {
                        UserIdentifier = userloginItem.UserIdentifier,
                        Activity = userloginItem.Activity.ToString(),
                        CreatedAt = userloginItem.CreatedAt,
                        IsActive = true,
                        IsDeleted = false,
                        TenantCode = tenantCode
                    })
                    .ToList();

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nVidYoEntitiesDataContext.UserLoginActivityHistoryRecords.AddRange(userLoginRecords);
                    nVidYoEntitiesDataContext.SaveChanges();
                    result = true;
                };
                return result;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get user login activity

        /// <summary>
        /// This method helps to retrieve list of user based on specified search paramters.
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<UserLoginActivityHistory> GetUserLogInActivityHistory(string userIdentifier, string tenantCode)
        {
            IList<UserLoginActivityHistoryRecord> activityRecords = null;

            IList<UserLoginActivityHistory> activityHistory = null;

            try
            {
                StringBuilder query = new StringBuilder();
                DateTime? FromDate = DateTime.UtcNow.AddMonths(-1);
                DateTime? ToDate = DateTime.UtcNow;
                if (FromDate != null && validationEngine.IsValidDate(DateTime.SpecifyKind(Convert.ToDateTime(FromDate), DateTimeKind.Utc)))
                {
                    DateTime dateTime = DateTime.SpecifyKind(Convert.ToDateTime(FromDate), DateTimeKind.Utc);
                    query.Append("CreatedAt >= DateTime(" + dateTime.Year + "," + dateTime.Month + "," + dateTime.Day + ","
                      + dateTime.Hour + "," + dateTime.Minute + "," + dateTime.Second + ") and ");
                }

                if (ToDate != null && validationEngine.IsValidDate(DateTime.SpecifyKind(Convert.ToDateTime(ToDate), DateTimeKind.Utc)))
                {
                    DateTime dateTime = DateTime.SpecifyKind(Convert.ToDateTime(ToDate), DateTimeKind.Utc);
                    if (dateTime != null)
                    {
                        query.Append("CreatedAt <= DateTime(" + dateTime.Year + "," + dateTime.Month + "," + dateTime.Day + ","
                          + dateTime.Hour + "," + dateTime.Minute + "," + dateTime.Second + ") and ");
                    }
                }

                query.Append(string.Format("UserIdentifier.Equals(\"{0}\") and TenantCode.Equals(\"{1}\")", userIdentifier, tenantCode));
                query.Append(" and IsDeleted.Equals(false)");

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    activityRecords = nVidYoEntitiesDataContext.UserLoginActivityHistoryRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (activityRecords.Count > 0)
                    {
                        activityHistory = new List<UserLoginActivityHistory>();
                        activityRecords?.ToList().ForEach(activityRecord =>
                        {
                            activityHistory.Add(new UserLoginActivityHistory()
                            {
                                UserIdentifier = activityRecord.UserIdentifier,
                                Activity = (Activity)(Enum.Parse(typeof(Activity), activityRecord.Activity.ToString())),
                                CreatedAt = activityRecord.CreatedAt,
                                TeanantCode = activityRecord.TenantCode,
                            });

                        });
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

            return activityHistory;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Check Duplicate User

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        public bool IsDuplicateUserEmailAndMobileNumber(IList<User> users, string operation, string tenantCode)
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
                    IList<UserRecord> userRecords = nVidYoEntitiesDataContext.UserRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
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
        /// <param name="searchParameter">User search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(UserSearchParameter searchParameter, string tenantCode)
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
                    queryString.Append(string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + " and ");
                }

                if (validationEngine.IsValidText(searchParameter.UserIdentifierToSkip))
                {
                    //queryString.Append("(" + string.Join("and ", searchParameter.UserIdentifierToSkip.ToString().Split(',').Select(item => string.Format(" !Id.Equals({0}) ", Convert.ToInt64(item)))) + ") and ");
                    queryString.Append("(" + string.Join("or ", searchParameter.UserIdentifierToSkip.ToString().Split(',').Select(item => string.Format("!Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(searchParameter.FirstName))
                {
                    // queryString.Append(queryString.Length > 0 ? " and " : string.Empty);
                    if (searchParameter.SearchMode == SearchMode.Equals)
                    {
                        // queryString.Append(string.Format("FirstName.Equals(\"{0}\") or LastName.Equals(\"{0}\") and ", searchParameter.FirstName, searchParameter.LastName));
                        queryString.Append("(" + string.Join("or ", searchParameter.FirstName.ToString().Split(',').Select(item => string.Format("FirstName.Equals(\"{0}\") or LastName.Equals(\"{0}\") ", item))) + ") and ");

                    }
                    else
                    {
                        //queryString.Append(string.Format("FirstName.Contains(\"{0}\") or LastName.Equals(\"{0}\") and ", searchParameter.FirstName, searchParameter.LastName));
                        queryString.Append("(" + string.Join("or ", searchParameter.FirstName.ToString().Split(',').Select(item => string.Format("FirstName.Contains(\"{0}\") or LastName.Contains(\"{0}\") ", item))) + ") and ");

                    }
                }

                if (validationEngine.IsValidText(searchParameter.EmailAddress))
                {
                    queryString.Append(queryString.Length > 0 ? " and " : string.Empty);
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

        #region Get Roles

        /// <summary>
        /// This method will be used to get roles
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        private IList<Role> GetRoles(long userIdentifier, bool? rolePrivilegeFlag, string tenantCode)
        {
            IList<Role> roles = new List<Role>();
            IList<RoleRecord> roleRecords = new List<RoleRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<UserRoleMapRecord> userRoleMapRecords = new List<UserRoleMapRecord>();
                    userRoleMapRecords = nVidYoEntitiesDataContext.UserRoleMapRecords.Where(x => x.UserId == userIdentifier).ToList();
                    if (userRoleMapRecords?.Count > 0)
                    {
                        string query = string.Join(" or ", userRoleMapRecords.Select(item => string.Format("Id.Equals({0})", item.RoleId.ToString())).ToList());
                        roleRecords = nVidYoEntitiesDataContext.RoleRecords.Where(query).ToList();
                    }

                    if (roleRecords?.Count > 0)
                    {
                        StringBuilder roleIdentifiers = new StringBuilder();
                        roleIdentifiers.Append("(" + string.Join(" or ", roleRecords.Select(item => string.Format("RoleIdentifier.Equals({0})", item.Id))) + ")");

                        IList<RolePrivilegeRecord> rolePrivilegeRecords = null;

                        if (rolePrivilegeFlag == true)
                        {
                            rolePrivilegeRecords = nVidYoEntitiesDataContext.RolePrivilegeRecords.Where(roleIdentifiers.ToString()).ToList();
                        }

                        roles = roleRecords.Select(roleRecord => new Role()
                        {
                            Identifier = roleRecord.Id,
                            Name = roleRecord.Name,
                            Description = roleRecord.Description,
                            IsActive = roleRecord.IsActive,
                            RolePrivileges = rolePrivilegeRecords?.Where(item => item.RoleIdentifier == roleRecord.Id)
                            .GroupBy(item => item.EntityName)
                            .Select(item => new RolePrivilege()
                            {
                                EntityName = item.Key,
                                RolePrivilegeOperations = item.Select(x => new RolePrivilegeOperation()
                                {
                                    Operation = x.Operation,
                                    IsEnabled = x.IsEnable
                                })
                                .ToList()
                            })
                            .ToList()
                        }).ToList();
                    }
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return roles;
        }

        #endregion

        //#region Get Country

        ///// <summary>
        ///// This method will be used to Get country
        ///// </summary>
        ///// <param name="countryIdentifier"></param>
        ///// <param name="tenantCode"></param>
        ///// <returns></returns>
        //private Country GetCountry(long? countryIdentifier, string tenantCode)
        //{
        //    IList<Country> countries = new List<Country>();
        //    IList<CountryRecord> countryRecords = new List<CountryRecord>();
        //    try
        //    {
        //        this.SetAndValidateConnectionString(tenantCode);
        //        if (countryIdentifier == 0)
        //        {
        //            countryIdentifier = 1;
        //        }
        //        using (NISEntities nVidiYoEntitiesDataContext = new NISEntities(this.connectionString))
        //        {
        //            countryRecords = nVidiYoEntitiesDataContext.CountryRecords.Where(x => x.Id == countryIdentifier).ToList();
        //        }

        //        if (countryRecords?.Count > 0)
        //        {
        //            countries = countryRecords.Select(countryRecord => new Country()
        //            {
        //                Identifier = countryRecord.Id,
        //                Code = countryRecord.Code,
        //                DialingCode = countryRecord.DialingCode,
        //                CountryName = countryRecord.Name,
        //                IsActive = countryRecord.IsActive
        //            }).ToList();
        //        }
        //        //else
        //        //{
        //        //    countries = null;
        //        //}
        //    }
        //    catch (SqlException)
        //    {
        //        throw new RepositoryStoreNotAccessibleException(tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return countries.First();
        //}

        //#endregion

        #region Map User and Role

        /// <summary>
        /// This method helps to map user and roles.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="userId"></param>
        /// <param name="tenantCode"></param>
        private void MapUserAndRole(IList<Role> roles, long userId, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    List<UserRoleMapRecord> userRoleMapRecord = new List<UserRoleMapRecord>();
                    IList<UserRoleMapRecord> userRoleMapRecords = nVidYoEntitiesDataContext.UserRoleMapRecords.Where(x => x.UserId == userId).Select(item => item).AsQueryable().ToList();

                    if (userRoleMapRecords.Count > 0)
                    {
                        nVidYoEntitiesDataContext.UserRoleMapRecords.RemoveRange(userRoleMapRecords);
                    }

                    roles.ToList().ForEach(role =>
                    {
                        userRoleMapRecord.Add(new UserRoleMapRecord
                        {
                            UserId = userId,
                            RoleId = role.Identifier
                        });
                    });

                    nVidYoEntitiesDataContext.UserRoleMapRecords.AddRange(userRoleMapRecord);
                    nVidYoEntitiesDataContext.SaveChanges();
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
