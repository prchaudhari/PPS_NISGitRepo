// <copyright file="AuthenticationManager.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region  References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class represents the authentication manager
    /// </summary>
    public class AuthenticationManager
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
        /// The utility.
        /// </summary>
        IUtility utility = new Utility();

        #endregion

        #region Constructor

        /// <summary>
        /// This is the consturctor for authentication manager
        /// </summary>
        /// <param name="unityContainer"></param>
        public AuthenticationManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.userRepository = this.unityContainer.Resolve<IUserRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method authenticates user
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns user if authenticated successfully otherwise an exception</returns>
        public User UserAuthenticate(string username, string password, string tenantCode)
        {
            User user = null;
            try
            {
                UserSearchParameter userSearchParameter = new UserSearchParameter()
                {
                    SortParameter = new SortParameter()
                    {
                        SortColumn = "Id",
                        SortOrder = SortOrder.Ascending
                    },
                    IsRolePrivilegesRequired = true,
                    EmailAddress = username,
                    IsActive = true
                };
                UserManager userManager = new UserManager(this.unityContainer);
                IList<User> users = userManager.GetUsers(userSearchParameter, tenantCode);

                if (users == null || users.Count <= 0 || !users.FirstOrDefault().IsActive)
                {
                    throw new UserNotFoundException(tenantCode);
                }
                user = users.FirstOrDefault();

                if (user.IsLocked != true)
                {
                    if (!userManager.IsAuthenticatedUser(user.EmailAddress, password, user.TenantCode))
                    {
                        if (user.NoofAttempts < 3)
                        {
                            userManager.UpdateUsersNoOfAttempts(user.Identifier.ToString(), user.TenantCode);
                            throw new InvalidUserPasswordException(user.TenantCode);
                        }
                        if (user.NoofAttempts == 3)
                        {
                            userManager.LockUser(user.Identifier, user.TenantCode);
                            throw new UserLockedException(tenantCode);
                        }
                    }

                    IList<UserLoginActivityHistory> userLoginDetails = new List<UserLoginActivityHistory>();
                    userLoginDetails.Add(new UserLoginActivityHistory()
                    {
                        UserIdentifier = user.Identifier.ToString(),
                        Activity = Activity.LogIn,
                        CreatedAt = DateTime.UtcNow
                    });

                    userManager.AddUserLogInActivityHistory(userLoginDetails, user.TenantCode);
                }
                else
                {
                    throw new UserLockedException(tenantCode);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return user;
        }

        #endregion
    }
}
