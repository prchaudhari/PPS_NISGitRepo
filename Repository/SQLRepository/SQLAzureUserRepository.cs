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
    using System.Linq;
    using System.Text;

    #endregion

    public class SQLAzureUserRepository : IUserRepository
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
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;


        #endregion

        #region Constructore

        #endregion

        #region Public Methods
        public SQLAzureUserRepository()
        {
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
        }

        public bool ActivateUser(long userIdentifier, string tenantCode)
        {
            throw new NotImplementedException();
        }

        public bool AddUsers(IList<User> users, string tenantCode)
        {
            throw new NotImplementedException();
        }

        public bool DeactivateUser(long userIdentifier, string tenantCode)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUsers(IList<User> users, string tenantCode)
        {
            throw new NotImplementedException();
        }

        public int GetUserCount(UserSearchParameter userSearchParameter, string tenantCode)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsers(UserSearchParameter userSearchParameter, string tenantCode)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUsers(IList<User> users, string tenantCode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods


        #endregion

    }
}
