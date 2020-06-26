// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    /// <summary>
    /// This class represents user constants
    /// </summary>
    public partial class ExceptionConstant
    {
        #region User

        /// <summary>
        /// The user exception section key
        /// </summary>
        public const string USER_EXCEPTION_SECTION = "UserException";

        /// <summary>
        /// The invalid user exception key
        /// </summary>
        //public const string INVALID_USER_EXCEPTION = "InvalidUserException";
        public const string INVALID_USER_EXCEPTION = "Invalid user";

        /// <summary>
        /// The invalid user search parameter exception key
        /// </summary>
        //public const string INVALID_USER_SEARCH_PARAMTER_EXCEPTION = "InvalidUserSearchParameterException";
        public const string INVALID_USER_SEARCH_PARAMTER_EXCEPTION = "Invalid user search parameter";

        /// <summary>
        /// The user not found exception key
        /// </summary>
        //public const string USER_NOT_FOUND_EXCEPTION = "UserNotFoundException";
        public const string USER_NOT_FOUND_EXCEPTION = "User not found";

        /// <summary>
        /// The user locked status found exception key
        /// </summary>
        //public const string USER_LOCKED_EXCEPTION = "UserLockedException";
        public const string USER_LOCKED_EXCEPTION = "Your account has been locked out. Please contact admin";

        /// <summary>
        /// The user locked status found exception key
        /// </summary>
        //public const string USER_LOCKED_EXCEPTION = "UserLockedException";
        public const string DEACTIVATED_USER_LOGIN= "Your account is deactivated, please contact admin";

        /// <summary>
        /// The user reference in team exception
        /// </summary>
        //public const string USER_LOCKED_EXCEPTION = "UserLockedException";
        public const string USER_REFERENCE_IN_TEAM_EXCEPTION = "User reference in team exception";

        /// <summary>
        /// The user reference in organization unit exception
        /// </summary>
        //public const string USER_REFERENCE_IN_ORGANIZATION_UNIT_EXCEPTION = "UserReferenceInOrganizationUnitException";
        public const string USER_REFERENCE_IN_ORGANIZATION_UNIT_EXCEPTION = "User reference in organization unit";

        /// <summary>
        /// The duplicate user found exception key
        /// </summary>
        //public const string DUPLICATE_USER_FOUND_EXCEPTION = "DuplicateUserFoundException";
        public const string DUPLICATE_USER_FOUND_EXCEPTION = "This Email Id already used in user";

        /// <summary>
        /// The duplicate user found exception key
        /// </summary>
        //public const string DUPLICATE_USER_FOUND_EXCEPTION = "DuplicateUserFoundException";
        public const string DUPLICATE_USER_EMAIL_FOUND_EXCEPTION = "This Email Id is already used in user";
        /// <summary>
        /// The duplicate user found exception key
        /// </summary>
        //public const string DUPLICATE_USER_FOUND_EXCEPTION = "DuplicateUserFoundException";
        public const string DUPLICATE_USER_MOBILE_NO_FOUND_EXCEPTION = "This Mobile No is already used in user";
        /// <summary>
        /// The invalid user password exception
        /// </summary>
        //public const string INVALID_USER_PASSWORD_EXCEPTION = "InvalidUserPasswordException";
        public const string INVALID_USER_PASSWORD_EXCEPTION = "Invalid user credentials";

        /// <summary>
        /// The invalid password exception
        /// </summary>
        //public const string INVALID_PASSWORD_EXCEPTION = "InvalidPasswordFormatException";
        public const string INVALID_PASSWORD_EXCEPTION = "Password does not meet complexity requirements";


        /// <summary>
        /// The already password exception
        /// </summary>
        //public const string Already_PASSWORD_EXCEPTION = "AlreadyPasswordFormatException";
        public const string ALREADY_PASSWORD_EXCEPTION = "You have already used that password, try another";

        #endregion
    }
}
