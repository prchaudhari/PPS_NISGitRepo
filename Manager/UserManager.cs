// <copyright file="RoleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Transactions;
    using Unity;
    using Websym.Core.EventManager;
    using Websym.Core.TenantManager;


    #endregion

    /// <summary>
    /// This class implements manager layer of user manager.
    /// </summary>
    public class UserManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The user repository.
        /// </summary>
        IUserRepository userRepository = null;

        /// <summary>
        /// The crypto manager
        /// </summary>
        private readonly ICryptoManager cryptoManager;


        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationUtility = null;

        /// <summary>
        /// The utility.
        /// </summary>
        IUtility utility = new Utility();

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public UserManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.userRepository = this.unityContainer.Resolve<IUserRepository>();
                this.cryptoManager = this.unityContainer.Resolve<ICryptoManager>();
                this.configurationUtility = new ConfigurationUtility(this.unityContainer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add

        /// <summary>
        /// This method helps to validate users and then add to database.
        /// </summary>
        /// <param name="users">List of user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool AddUsers(IList<User> users, string tenantCode, bool isRegisterUser = true)
        {
            bool result = false;
            try
            {
                this.IsValidusers(users, tenantCode);
                this.IsDuplicateEmailOrContactNumber(users, tenantCode);
                IList<UserLogin> userLoginDetails = null;

                result = this.userRepository.AddUsers(users, tenantCode);

                if (isRegisterUser)
                {
                    userLoginDetails = new List<UserLogin>();
                    userLoginDetails = users.Select(userItem => new UserLogin()
                    {
                        UserIdentifier = userItem.EmailAddress,
                        UserPassword = this.cryptoManager.EncryptPassword(this.GeneratePassword()),
                        UserEncryptedPassword = this.cryptoManager.EncryptPassword(this.GeneratePassword()),
                        IsSystemGenerated = true
                    })
                    .ToList();

                    // adding user password mapping
                    this.userRepository.AddUsersCredential(userLoginDetails, tenantCode);

                    // adding user password mapping in to history table
                    this.userRepository.AddUsersCredentialHistory(userLoginDetails, tenantCode);

                    // sending mail (Send grid api used)
                    // this.UsersMailManager(users, userLoginDetails, tenantCode);
                    //  transactionScope.Complete();
                    //};


                    try
                    {
                        //this.SendNotification(users, userLoginDetails, EntityType.User.ToString(), EventLabelType.UserAdd.ToString(), (int)NotificationEvents.UserAdd, true, tenantCode);

                        foreach (User user in users)
                        {
                            string param = this.cryptoManager.Encrypt(string.Format("{0}:{1}", user.EmailAddress, userLoginDetails.First().UserEncryptedPassword));
                            MailMessage mail = new MailMessage();
                            mail.To.Add(user.EmailAddress);
                            mail.Subject = ConfigurationManager.AppSettings[ModelConstant.NEWLYADDEDUSERMAILSUBJECT];
                            mail.Body = string.Format(ConfigurationManager.AppSettings[ModelConstant.NEWLYADDEDUSERMAILMESSAGE], user.FirstName, "<a href='" + ConfigurationManager.AppSettings[ModelConstant.CHANGEPASSWORDLINK] + param + "'>click here</a>.");
                            mail.IsBodyHtml = true;
                            IUtility iUtility = new Utility();
                            iUtility.SendMail(mail, string.Empty, 0, string.Empty, tenantCode);
                        }
                    }
                    catch
                    {

                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SendActivationLinkToGroupManager(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {
                IList<UserLogin> userLoginDetails = null;


                userLoginDetails = new List<UserLogin>();
                userLoginDetails = users.Select(userItem => new UserLogin()
                {
                    UserIdentifier = userItem.EmailAddress,
                    UserPassword = this.cryptoManager.EncryptPassword(this.GeneratePassword()),
                    UserEncryptedPassword = this.cryptoManager.EncryptPassword(this.GeneratePassword()),
                    IsSystemGenerated = true
                })
                .ToList();

                // adding user password mapping
                this.userRepository.AddUsersCredential(userLoginDetails, tenantCode);

                // adding user password mapping in to history table
                this.userRepository.AddUsersCredentialHistory(userLoginDetails, tenantCode);

                // sending mail (Send grid api used)
                // this.UsersMailManager(users, userLoginDetails, tenantCode);
                //  transactionScope.Complete();
                //};


                try
                {
                    //this.SendNotification(users, userLoginDetails, EntityType.User.ToString(), EventLabelType.UserAdd.ToString(), (int)NotificationEvents.UserAdd, true, tenantCode);

                    foreach (User user in users)
                    {
                        string param = this.cryptoManager.Encrypt(string.Format("{0}:{1}", user.EmailAddress, userLoginDetails.First().UserEncryptedPassword));
                        MailMessage mail = new MailMessage();
                        mail.To.Add(user.EmailAddress);
                        mail.Subject = ConfigurationManager.AppSettings[ModelConstant.NEWLYADDEDUSERMAILSUBJECT];
                        mail.Body = string.Format(ConfigurationManager.AppSettings[ModelConstant.NEWLYADDEDUSERMAILMESSAGE], user.FirstName, "<a href='" + ConfigurationManager.AppSettings[ModelConstant.CHANGEPASSWORDLINK] + param + "'>click here</a>.");
                        mail.IsBodyHtml = true;
                        IUtility iUtility = new Utility();
                        iUtility.SendMail(mail, string.Empty, 0, string.Empty, tenantCode);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// This method helps to validate users and then update to database.
        /// </summary>
        /// <param name="users">List of user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool UpdateUsers(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidusers(users, tenantCode);
                this.IsDuplicateEmailOrContactNumber(users, tenantCode);
                result = this.userRepository.UpdateUsers(users, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region Delete

        /// <summary>
        /// This method helps to delete users from database.
        /// </summary>
        /// <param name="users">List of user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool DeleteUsers(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.CheckUserDependency(users, tenantCode);
                result = this.userRepository.DeleteUsers(users, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region Get

        /// <summary>
        /// This method helps to get specified users from database using given user's search parameter.
        /// </summary>
        /// <param name="searchParameter">
        /// Search parameter to search specified user.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param> 
        /// <returns>
        /// Returns a list of user if any found otherwise it will return enpty list.
        /// </returns>
        public IList<User> GetUsers(UserSearchParameter searchParameter, string tenantCode)
        {
            try
            {
                if (searchParameter == null)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);

                try
                {
                    searchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                    throw invalidSearchParameterException;
                }
                IList<User> users = this.userRepository.GetUsers(searchParameter, tenantCode);

                return users;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to get specified users from database using given user's search parameter.
        /// </summary>
        /// <param name="searchParameter">
        /// Search parameter to search specified user.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param> 
        /// <returns>
        /// Returns a list of user if any found otherwise it will return enpty list.
        /// </returns>
        public IList<User> GetUserToAuthentication(UserSearchParameter searchParameter, string tenantCode)
        {
            try
            {
                if (searchParameter == null)
                {
                    throw new NullArgumentException(tenantCode);
                }
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    searchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                    throw invalidSearchParameterException;
                }
                return this.userRepository.GetUserToAuthentication(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region User Authentication

        /// <summary>
        /// This method will validate the request and then send call to repository.
        /// </summary>
        /// <param name="newPassword">
        /// New password string.
        /// </param>
        /// <param name="encryptedText">
        /// Encrypted string.
        /// </param>
        /// <returns>
        /// If successfully updated password, it will return true.
        /// if any error will occured, it will throw exception.
        /// </returns>
        public bool ChangePassword(string newPassword, string encryptedText, string tenantCode)
        {
            bool result = false;
            try
            {
                string decryptedText = this.cryptoManager.Decrypt(encryptedText);

                if (!decryptedText.Contains(":"))
                {
                    throw new InvalidEncryptedDataException(tenantCode);
                }


                string[] values = decryptedText.Split(':');
                UserSearchParameter searchParameter = new UserSearchParameter()
                {
                    SortParameter = new SortParameter()
                    {
                        SortColumn = "Id"
                    },
                    EmailAddress = values[0]
                };

                User user = this.GetUsers(searchParameter, tenantCode).FirstOrDefault();
                if (user == null)
                {
                    throw new UserNotFoundException(tenantCode);
                }

                #region Validate Password complexity

                //Password should contain atleast one uppar,one lower,one special character & one digit
                if (!this.IsValidPassword(newPassword, tenantCode))
                {
                    throw new InvalidPasswordFormatException(tenantCode);
                }

                //Last used password can't be reused
                string encryptedPassword = this.cryptoManager.EncryptPassword(newPassword);
                if (!this.userRepository.IsPasswordHistoryValidation(user.Identifier.ToString(), encryptedPassword, user.TenantCode))
                {
                    throw new LastUsedPasswordException(tenantCode);
                }

                #endregion



                if (!this.userRepository.IsAuthenticatedUser(user.EmailAddress, values[1], tenantCode))
                {
                    throw new InvalidUserPasswordException(tenantCode);
                }

                // calling current class method to encrypt password and then change it.
                result = this.ChangePassword(new UserLogin() { UserIdentifier = user.EmailAddress, UserPassword = newPassword, TeanantCode = user.TenantCode }, tenantCode);
                IList<UserLoginActivityHistory> histories = this.GetUserLogInActivityHistory(user.Identifier.ToString(), tenantCode);
                if (histories == null)
                {
                    this.ActivateUser(user.Identifier, tenantCode);
                }
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will update password when logged in user send a request to change password.
        /// </summary>
        /// <param name="newPassword">
        /// New password string.
        /// </param>
        /// <param name="encryptedText">
        /// Encrypted string.
        /// </param>
        /// <returns>
        /// If successfully updated password, it will return true.
        /// if any error will occured, it will throw exception.
        /// </returns>
        public bool ChangePassword(string userEmail, string oldPassword, string newPassword, string tenantCode)
        {
            try
            {
                UserSearchParameter searchParameter = new UserSearchParameter();
                searchParameter.SortParameter.SortColumn = "Id";
                searchParameter.EmailAddress = userEmail;

                User user = this.userRepository.GetUsers(searchParameter, tenantCode).FirstOrDefault();

                if (user == null)
                {
                    throw new UserNotFoundException(tenantCode);
                }

                #region Validate Password complexity

                //Password should contain atleast one uppar,one lower,one special character & one digit
                if (!this.IsValidPassword(newPassword, tenantCode))
                {
                    throw new InvalidPasswordFormatException(tenantCode);
                }
                string encryptedPassword = this.cryptoManager.EncryptPassword(newPassword);
                //Last used password can't be reused
                if (!this.userRepository.IsPasswordHistoryValidation(user.Identifier.ToString(), encryptedPassword, user.TenantCode))
                {
                    throw new LastUsedPasswordException(tenantCode);
                }

                #endregion

                if (!this.IsAuthenticatedUser(user.EmailAddress, oldPassword, tenantCode))
                {
                    throw new InvalidUserPasswordException(tenantCode);
                }

                //This flag is added for password reset by admin provision, 
                //once admin user reset the user password then respecive user will get new password by mail, then this flag will set to true
                //and this flag value will check as at the time of login, if true then system redirect to change password screen on respective users login
                //So he can change the password as they wants and then update password reset by admin flag value to false.
                if (user.IsPasswordResetByAdmin == true)
                {
                    user.IsPasswordResetByAdmin = false;
                    var contactnumber = user.ContactNumber.Split(new Char[] { '-' })[1];
                    user.ContactNumber = contactnumber;
                    IList<User> users = new List<User>();
                    users.Add(user);
                    this.UpdateUsers(users, tenantCode);
                }

                return this.ChangePassword(new UserLogin() { UserIdentifier = user.EmailAddress, UserPassword = newPassword, TeanantCode = user.TenantCode }, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to validate user login
        /// </summary>
        /// <param name="userIdentifier">
        /// User identifier.
        /// </param>
        /// <param name="password">
        /// user password.
        /// </param>
        /// <returns>
        /// If password is correct then it will return true otherwise false.
        /// </returns>
        public bool IsAuthenticatedUser(string userIdentifier, string plainPassword, string tenantCode)
        {
            try
            {
                string encryptedPassword = this.cryptoManager.EncryptPassword(plainPassword);
                return this.userRepository.IsAuthenticatedUser(userIdentifier, encryptedPassword, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will sent a mail for reset user's password.
        /// </summary>
        /// <param name="userEmail">
        /// User email address.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code.
        /// </param>
        /// <returns>
        /// If successfully compeleted, it will return true.
        /// </returns>
        public bool ResetUserPassword(string userEmail, string tenantCode)
        {
            bool result = false;
            try
            {
                UserSearchParameter searchParameter = new UserSearchParameter()
                {
                    SortParameter = new SortParameter()
                    {
                        SortColumn = "Id"
                    },
                    EmailAddress = userEmail
                };

                User user = this.GetUsers(searchParameter, tenantCode).FirstOrDefault();
                if (user == null || !user.IsActive)
                {
                    throw new UserNotFoundException(tenantCode);
                }

                UserLogin userLoginDetail = this.userRepository.GetUserAuthenticationDetail(user.EmailAddress, tenantCode);

                if (userLoginDetail == null)
                {
                    throw new UserNotFoundException(tenantCode);
                }

                string param = this.cryptoManager.Encrypt(string.Format("{0}:{1}", user.EmailAddress, userLoginDetail.UserEncryptedPassword));

                MailMessage mail = new MailMessage();
                mail.To.Add(user.EmailAddress);
                mail.Subject = ConfigurationManager.AppSettings[ModelConstant.USERFORGOTPASSWORDSUBJECT];
                mail.Body = string.Format(ConfigurationManager.AppSettings[ModelConstant.USERFORGOTPASSWORDMESSAGE], user.FirstName, "<a href='" + ConfigurationManager.AppSettings[ModelConstant.CHANGEPASSWORDLINK] + param + "'>click here</a>.");
                mail.IsBodyHtml = true;
                IUtility iUtility = new Utility();
                iUtility.SendMail(mail, string.Empty, 0, string.Empty, tenantCode);

                result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will sent a mail of system generated password to user.
        /// </summary>
        /// <param name="userEmail">
        /// User email address.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code.
        /// </param>
        /// <returns>
        /// If successfully compeleted, it will return true.
        /// </returns>
        public bool ResetUserPasswordByMail(string userEmail, string tenantCode)
        {
            try
            {
                UserSearchParameter searchParameter = new UserSearchParameter()
                {
                    SortParameter = new SortParameter()
                    {
                        SortColumn = "Id"
                    },
                    EmailAddress = userEmail
                };
                var user = this.GetUsers(searchParameter, tenantCode).FirstOrDefault();
                if (user == null)
                {
                    throw new UserNotFoundException(tenantCode);
                }

                //To set password reset by admin value to true which will check on login for this user,
                //If true then will redirect to change password screen, otherwise the respective landing page after login
                user.IsPasswordResetByAdmin = true;
                var contactnumber = user.ContactNumber.Split(new Char[] { '-' })[1];
                user.ContactNumber = contactnumber;
                IList<User> users = new List<User>();
                users.Add(user);
                var result = this.UpdateUsers(users, tenantCode);
                if (result == true)
                {
                    var newPassword = this.CreatePassword(10);
                    var res = this.ChangePassword(new UserLogin() { UserIdentifier = user.EmailAddress, UserPassword = newPassword, TeanantCode = user.TenantCode, IsSystemGenerated = true }, tenantCode);
                    MailMessage mail = new MailMessage();
                    mail.To.Add(user.EmailAddress);
                    mail.Subject = ConfigurationManager.AppSettings[ModelConstant.USERFORGOTPASSWORDSUBJECT];
                    mail.Body = string.Format(ConfigurationManager.AppSettings[ModelConstant.SENDPASSWORDMAILTOUSERMESSAGE], user.FirstName, "Password: " + newPassword);
                    mail.IsBodyHtml = true;
                    IUtility iUtility = new Utility();
                    iUtility.SendMail(mail, string.Empty, 0, string.Empty, tenantCode);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region UpdateUsersNoOfAttempts

        /// <summary>
        /// This method helps to update no odf attempts users from database.
        /// </summary>
        /// <param name="user">user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully updtaed, it will return true.
        /// </returns>
        public bool UpdateUsersNoOfAttempts(string userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.userRepository.UpdateUsersNoOfAttempts(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region Update User Locked status

        /// <summary>
        /// This method helps to update user locked status
        /// </summary>
        /// <param name="user">user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully updated, it will return true.
        /// </returns>
        public bool LockUser(long userIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.userRepository.LockUser(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region UnlockUser

        /// <summary>
        /// This method helps to active user.
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <returns></returns>
        public bool UnlockUser(long userIdentifier, string tenantCode)
        {
            try
            {
                return this.userRepository.UnlockUser(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region user Login Activity Add

        public bool AddUserLogInActivityHistory(IList<UserLoginActivityHistory> userLoginDetails, string tenantCode)
        {
            return this.userRepository.AddUserLogInActivityHistory(userLoginDetails, tenantCode);
        }

        #endregion

        #region user Lgin Activity Get

        public IList<UserLoginActivityHistory> GetUserLogInActivityHistory(string userIdentifier, string tenantCode)
        {
            return this.userRepository.GetUserLogInActivityHistory(userIdentifier, tenantCode);
        }

        #endregion

        #region Get Count

        /// <summary>
        /// This method helps to get users count.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of usesrs
        /// </returns>
        public int GetUserCount(UserSearchParameter userSearchParameter, string tenantCode)
        {
            int userCount = 0;
            try
            {
                userCount = this.userRepository.GetUserCount(userSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return userCount;
        }

        #endregion

        #region Activate

        /// <summary>
        /// This method helps to active user.
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <returns></returns>
        public bool ActivateUser(long userIdentifier, string tenantCode)
        {
            try
            {
                return this.userRepository.ActivateUser(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This mehod helps to deactive user.
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <returns></returns>
        public bool DeActivateUser(long userIdentifier, string tenantCode)
        {
            try
            {
                this.CheckUserDependency(new List<User> { new User() { Identifier = userIdentifier } }, tenantCode);
                return this.userRepository.DeactivateUser(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Is Duplicate User

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
            try
            {
                return this.userRepository.IsDuplicateUserEmailAndMobileNumber(users, operation, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method will update user's password.
        /// </summary>
        /// <param name="newPassword">
        /// New password string.
        /// </param>
        /// <param name="encryptedText">
        /// Encrypted string.
        /// </param>
        /// <returns>
        /// If successfully updated password, it will return true.
        /// if any error will occured, it will throw exception.
        /// </returns>
        private bool ChangePassword(UserLogin userLoginDetail, string tenantCode)
        {
            try
            {
                bool result = false;

                userLoginDetail.UserEncryptedPassword = this.cryptoManager.EncryptPassword(userLoginDetail.UserPassword);

                result = this.userRepository.ChangePassword(userLoginDetail, tenantCode);

                IList<UserLogin> userLoginDetails = new List<UserLogin>();
                userLoginDetails.Add(userLoginDetail);

                // adding user password mapping in to history table
                this.userRepository.AddUsersCredentialHistory(userLoginDetails, userLoginDetail.TeanantCode);

                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method is responsible for validate users.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantCode"></param>
        private void IsValidusers(IList<User> users, string tenantCode)
        {
            try
            {
                if (users?.Count > 0)
                {
                    InvalidUserException invalidUserException = new InvalidUserException(tenantCode);
                    users.ToList().ForEach(item =>
                    {
                        try
                        {
                            item.IsValid();
                            if (string.IsNullOrEmpty(item.Image))
                            {
                                //string base64Image = "data:image/jpg;base64,";
                                //string path1 = System.Web.HttpContext.Current.Server.MapPath("~/images/user-avatar.jpg");

                                //byte[] imageArray = System.IO.File.ReadAllBytes(path1);
                                //base64Image += Convert.ToBase64String(imageArray);
                                //item.Image = base64Image;
                            }
                        }
                        catch (Exception ex)
                        {
                            invalidUserException.Data.Add(item.FirstName, ex.Data);
                        }
                    });

                    if (invalidUserException.Data.Count > 0)
                    {
                        throw invalidUserException;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate user in the list
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateEmailOrContactNumber(IList<User> users, string tenantCode)
        {
            int duplicateUserCount = 0;
            try
            {
                if (users?.Count > 0)
                {
                    duplicateUserCount = users.GroupBy(c => new
                    {
                        c.EmailAddress,
                        c.ContactNumber,
                    }).Where(g => g.Count() > 1).Count();
                    if (duplicateUserCount > 0)
                    {
                        throw new DuplicateUserFoundException(tenantCode);
                    }
                    else
                    {
                        duplicateUserCount = users.GroupBy(c => c.EmailAddress).Where(g => g.Count() > 1).Count();
                        if (duplicateUserCount > 0)
                        {
                            throw new DuplicateUserEmailAddressFoundException(tenantCode);
                        }
                        else
                        {
                            duplicateUserCount = users.GroupBy(p => p.ContactNumber).Where(g => g.Count() > 1).Count();
                            if (duplicateUserCount > 0)
                            {
                                throw new DuplicateUserMobileNumberFoundException(tenantCode);
                            }
                        }
                    }


                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will return the system generated random number 
        /// </summary>
        /// <returns>
        /// It will return rendom string.
        /// </returns>
        private string GeneratePassword()
        {
            //// Set password as random number and encrypt it
            Random random = new Random();
            return Convert.ToString(random.Next());
        }

        /// <summary>
        /// This method helps to check user dependency on other entity.
        /// </summary>
        /// <param name="users">
        /// The users
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void CheckUserDependency(IList<User> users, string tenantCode)
        {
            try
            {
                string userIdentifiers = string.Join(",", users.Select(user => user.Identifier.ToString()).ToList());
                users.ToList().ForEach(item =>
                {
                    if (item.IsGroupManager)
                    {
                        ClientSearchParameter clientSearchPaarmeter = new ClientSearchParameter();
                        clientSearchPaarmeter.SortParameter = new SortParameter();
                        clientSearchPaarmeter.SortParameter.SortColumn = "TenantCode";
                        clientSearchPaarmeter.PrimaryEmailAddress = item.EmailAddress;
                        var clients = new ClientManager(this.unityContainer).GetClients(clientSearchPaarmeter, ModelConstant.DEFAULT_TENANT_CODE);
                        if (clients != null || clients?.Count > 0)
                        {
                            throw new UserReferenceInTenantGroupException(item.TenantCode);

                        }
                    }
                });

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This is responsible for validate password (password shold be one capital,one small,one special & one number
        /// </summary>
        /// <param name="password"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        private bool IsValidPassword(string password, string tenantCode)
        {
            try
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasMinimum8Chars = new Regex(@".{8,}");
                Regex specialCharacter = new Regex(@"[~`!@#$%^&*()-+=|\{}':;.,<>/?]");
                bool isValidated = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasLowerChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password) && specialCharacter.IsMatch(password);
                return isValidated;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will return the system generated random password
        /// </summary>
        /// <returns>
        /// It will return rendom password string which contains atleast 1 upper case letter, 1 lower case letter, 1 special character and 1 digit
        /// </returns>
        private string CreatePassword(int length)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "1234567890";
            const string specialCharacters = "~!@#$*?";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            int upperchars = 2;
            int specialChars = 1;
            int numberic = 2;
            int lowerchars = length - upperchars - specialChars - numberic;

            while (0 < upperchars--)
            {
                res.Append(letters[rnd.Next(letters.Length)].ToString().ToUpper());
            }
            while (0 < numberic--)
            {
                res.Append(numbers[rnd.Next(numbers.Length)]);
            }
            while (0 < specialChars--)
            {
                res.Append(specialCharacters[rnd.Next(specialCharacters.Length)]);
            }
            while (0 < lowerchars--)
            {
                res.Append(letters[rnd.Next(letters.Length)]);
            }

            return res.ToString();
        }

        /// <summary>
        /// This method will manage subject, body and then send mail.
        /// </summary>
        /// <param name="users">
        /// User object.
        /// </param>
        private void UsersMailManager(IList<User> users, IList<UserLogin> userLoginDetails, string tenantCode)
        {
            try
            {
                IUtility iUtility = new Utility();
                foreach (User user in users)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(user.EmailAddress);
                    string param = this.cryptoManager.Encrypt(user.EmailAddress + ":" + userLoginDetails.Where(item => item.UserIdentifier == user.EmailAddress.ToString()).FirstOrDefault().UserPassword);
                    mail.Subject = ConfigurationManager.AppSettings[ModelConstant.NEWLYADDEDUSERMAILSUBJECT];
                    mail.Body = ConfigurationManager.AppSettings[ModelConstant.NEWLYADDEDUSERMAILMESSAGE] + " <br/> <a href='" + ConfigurationManager.AppSettings[ModelConstant.CHANGEPASSWORDLINK] + param + "'> Click here </a> to generate your password.";
                    iUtility.SendMail(mail, "", 0, "", tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
                //Debug.WriteLine("Email has not been sent.");
                //Debug.WriteLine(exception.Message);
            }
        }

        //private async void SendNotification(IList<User> users, IList<UserLogin> userLoginDetails, string entityName, string eventName, int eventCode, bool isUserAdd, string tenantCode)
        //{
        //    try
        //    {
        //        #region Get Notification Setup

        //        NotificationEventSearchParameter notificationSearchParameter = new NotificationEventSearchParameter()
        //        {
        //            SortParameter = new SortParameter()
        //            {
        //                SortColumn = ModelConstant.SORT_COLUMN_NAME
        //            },
        //            Name = eventName
        //        };

        //        NotificationEvent notificationEvent = new NotificationEventManager(this.unityContainer).GetNotificationEvents(notificationSearchParameter, tenantCode)?.FirstOrDefault();
        //        if (notificationEvent == null)
        //        {
        //            IList<NotificationEvent> notificationEvents = new List<NotificationEvent>();
        //            notificationEvents.Add(new NotificationEvent()
        //            {
        //                EventLabel = "UserAdd",
        //                Name = "UserAdd",
        //                Description = "",
        //                Value = "",
        //                IsEmailEnabled = true,
        //                IsSMSEnabled = true,
        //                IsDeleted = false,
        //                IsValueRequired = false,
        //                Title = "lblNotificationSetupUIUser",
        //                EventSequenceNumber = 1,
        //                Isvisible = false
        //            });

        //            notificationEvents.Add(new NotificationEvent()
        //            {
        //                EventLabel = "ForgotPassword",
        //                Name = "ForgotPassword",
        //                Description = "",
        //                Value = "",
        //                IsEmailEnabled = true,
        //                IsSMSEnabled = true,
        //                IsDeleted = false,
        //                IsValueRequired = false,
        //                Title = "lblNotificationSetupUIUser",
        //                EventSequenceNumber = 2,
        //                Isvisible = false
        //            });

        //            bool res = new NotificationEventManager(this.unityContainer).AddNotificationEvents(notificationEvents, tenantCode);
        //            if (res == true)
        //            {
        //                //get notification event
        //                notificationEvent = new NotificationEventManager(this.unityContainer).GetNotificationEvents(notificationSearchParameter, tenantCode)?.FirstOrDefault();
        //            }
        //            else
        //            {
        //                throw new NotificationEventNotFoundException(tenantCode);
        //            }

        //        }

        //        #endregion

        //        string[] deliveryModes = Enum.GetNames(typeof(SubscrptionDeliveryMode))?.Where(item => item != SubscrptionDeliveryMode.None.ToString()).ToArray();
        //        Websym.Core.EventManager.EventSearchParameter eventSearchparameter = new Websym.Core.EventManager.EventSearchParameter()
        //        {
        //            ComponentCode = ModelConstant.COMPONENTCODE,
        //            //EntityName = entityName,
        //            //EventName = eventName,
        //            SortParameter = new Websym.Core.EventManager.SortParameter()
        //            {
        //                SortColumn = "EntityName"
        //            }
        //        };

        //        users.ToList().ForEach(user =>
        //        {
        //            deliveryModes.ToList().ForEach(deliveryModeItem =>
        //            {
        //                if (deliveryModeItem == "HTMLEmail")
        //                {
        //                    SubscrptionDeliveryMode deliveryMode;
        //                    Enum.TryParse<SubscrptionDeliveryMode>(deliveryModeItem, out deliveryMode);

        //                    string param = this.cryptoManager.Encrypt(user.EmailAddress + ":" + userLoginDetails.Where(item => item.UserIdentifier == user.EmailAddress).FirstOrDefault().UserPassword);
        //                    if (isUserAdd)
        //                    {
        //                        #region Add Subscription

        //                        string contactNo = "";

        //                        if (user.ContactNumber.Split('-').Length == 2)
        //                        {
        //                            contactNo = user.ContactNumber.Split('-')[1];
        //                        }
        //                        else
        //                        {
        //                            contactNo = user.ContactNumber;
        //                        }

        //                        new ConfigurationUtility(this.unityContainer).AddUserNotificationSubscription(eventSearchparameter, deliveryMode, user.Identifier.ToString(), contactNo, user.EmailAddress, tenantCode);

        //                        #endregion
        //                    }


        //                    #region Send Notification SMS/Email  

        //                    if ((SubscrptionDeliveryMode.SMS.ToString() == deliveryModeItem && notificationEvent.IsSMSEnabled) || (SubscrptionDeliveryMode.HTMLEmail.ToString() == deliveryModeItem && notificationEvent.IsEmailEnabled))
        //                    {

        //                        List<EventData> eventDataList = new List<EventData>();
        //                        eventDataList.Add(new EventData() { EventKey = ModelConstant.HTML_LINK, EventValue = ConfigurationManager.AppSettings[ModelConstant.CHANGE_PASSWORD_LINK] + param });
        //                        if (SubscrptionDeliveryMode.HTMLEmail.ToString() == deliveryModeItem)
        //                        {
        //                            eventDataList.Add(new EventData() { EventKey = ModelConstant.USER_FIRST_NAME, EventValue = user.FirstName });
        //                        }
        //                        EventContext eventContext = new EventContext()
        //                        {
        //                            EntityName = entityName,
        //                            EventName = eventName,
        //                            //EventCode = events.FirstOrDefault().EventCode,
        //                            TenantCode = tenantCode,
        //                            UserIdentifier = user.Identifier.ToString(),
        //                            ContextValues = eventDataList,
        //                            ComponentCode = "nVidYo",
        //                            EventCode = eventCode
        //                        };

        //                        //bool successfullNotified = new ConfigurationUtility(unityContainer).SendNotification(eventContext, deliveryMode, tenantCode);
        //                        //if (!successfullNotified)
        //                        //{
        //                        //    throw new Exception("Failed to notify.");
        //                        //}
        //                    }

        //                    #endregion
        //                }

        //            });
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion
    }
}
