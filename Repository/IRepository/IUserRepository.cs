// <copyright file="IUserRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System.Collections.Generic;

namespace nIS
{
    public interface IUserRepository
    {
        /// <summary>
        /// This method adds the specified list of users in user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are added successfully, else false.
        /// </returns>
        bool AddUsers(IList<User> users, string tenantCode);

        /// <summary>
        /// This method updates the specified list of users in user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are updated successfully, else false.
        /// </returns>
        bool UpdateUsers(IList<User> users, string tenantCode);


        /// <summary>
        /// This method deletes the specified list of users from user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are deleted successfully, else false.
        /// </returns>
        bool DeleteUsers(IList<User> users, string tenantCode);

        /// <summary>
        /// This method gets the specified list of users from user repository.
        /// </summary>
        /// <param name="userSearchParameter">The user search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of Users
        /// </returns>
        IList<User> GetUsers(UserSearchParameter userSearchParameter, string tenantCode);

        /// <summary>
        /// This method's reference helps to update user's password to database.
        /// </summary>
        /// <param name="userLoginDetail">User login object.</param>
        /// <param name="tenantCode">The tenant code.</param> 
        /// <returns>It will return true if successfully updated password.</returns>
        bool ChangePassword(UserLogin userLoginDetail, string tenantCode);

        /// <summary>
        /// This method,s reference will validate user login.
        /// </summary>
        /// <param name="userIdentifier">User identifier</param>
        /// <param name="password">User password</param>
        /// <returns>
        /// If password is correct then it will return true otherwise false.
        /// </returns>
        bool IsAuthenticatedUser(string userIdentifier, string password, string tenantCode);

        /// <summary>
        /// This method's reference will add users credential.
        /// </summary>
        /// <param name="userLoginDetails">
        /// User login object.
        /// </param>
        void AddUsersCredential(IList<UserLogin> userLoginDetails, string tenantCode);

        /// <summary>
        /// This method's reference will helps to get user loagin detail.
        /// </summary>
        /// <param name="userIdentifier">
        /// User identifier.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return UserLogin object.
        /// </returns>
        UserLogin GetUserAuthenticationDetail(string userIdentifier, string tenantCode);

        /// <summary>
        /// This is responsible for update no of attaempts of user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool UpdateUsersNoOfAttempts(string userIdentifier, string tenantCode);

        /// <summary>
        ///  This is responsible for update locked status of user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool LockUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to unlock the users
        /// </summary>
        /// <param name="userIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer activated successfully false otherwise</returns>
        bool UnlockUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to activate the users
        /// </summary>
        /// <param name="userIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer activated successfully false otherwise</returns>
        bool ActivateUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the users
        /// </summary>
        /// <param name="userIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer deactivated successfully false otherwise</returns>
        bool DeactivateUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to get user count
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Role count</returns>
        int GetUserCount(UserSearchParameter userSearchParameter, string tenantCode);

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        bool IsDuplicateUserEmailAndMobileNumber(IList<User> users, string operation, string tenantCode);

        /// <summary>
        /// This is responsible for password history validation
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="newPassword"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool IsPasswordHistoryValidation(string userIdentifier, string newPassword, string tenantCode);

        /// <summary>
        /// This method's reference will add users credential.
        /// </summary>
        /// <param name="userLoginDetails">
        /// User login object.
        /// </param>
        void AddUsersCredentialHistory(IList<UserLogin> userLoginDetails, string tenantCode);

        /// <summary>
        /// This is responsible for adding user login activity
        /// </summary>
        /// <param name="userLoginDetails"></param>
        /// <param name="tenantCode"></param>
        bool AddUserLogInActivityHistory(IList<UserLoginActivityHistory> userLoginDetails, string tenantCode);

        /// <summary>
        /// This method helps to retrieve list of user login activity
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        IList<UserLoginActivityHistory> GetUserLogInActivityHistory(string userIdentifier, string tenantCode);

    }
}
