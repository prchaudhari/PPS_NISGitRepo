// <copyright file="StatementManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;
    #endregion

    public class StatementManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The Statement repository.
        /// </summary>
        IStatementRepository StatementRepository = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public StatementManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.StatementRepository = this.unityContainer.Resolve<IStatementRepository>();
                this.validationEngine = new ValidationEngine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will call add Statements method of repository.
        /// </summary>
        /// <param name="Statements">Statements are to be add.</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddStatements(IList<Statement> Statements, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidStatements(Statements, tenantCode);
                this.IsDuplicateStatement(Statements, tenantCode);
                result = this.StatementRepository.AddStatements(Statements, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update Statements method of repository
        /// </summary>
        /// <param name="Statements">Statements are to be update.</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if roles updated successfully, false otherwise.
        /// </returns>
        public bool UpdateStatements(IList<Statement> Statements, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidStatements(Statements, tenantCode);
                this.IsDuplicateStatement(Statements, tenantCode);
                result = this.StatementRepository.UpdateStatements(Statements, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete Statements method of repository
        /// </summary>
        /// <param name="StatementIdentifier">Statement iddentifier</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if Statements deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteStatements(long StatementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementRepository.DeleteStatements(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get Statements method of repository.
        /// </summary>
        /// <param name="StatementSearchParameter">The Statement search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns Statements if found for given parameters, else return null
        /// </returns>
        public IList<Statement> GetStatements(StatementSearchParameter StatementSearchParameter, string tenantCode)
        {
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    StatementSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                StatementSearchParameter.StartDate = this.validationEngine.IsValidDate(StatementSearchParameter.StartDate) ? StatementSearchParameter.StartDate.ToLocalTime() : StatementSearchParameter.StartDate;
                StatementSearchParameter.EndDate = this.validationEngine.IsValidDate(StatementSearchParameter.EndDate) ? StatementSearchParameter.EndDate.ToLocalTime() : StatementSearchParameter.EndDate;

                return this.StatementRepository.GetStatements(StatementSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to get count of Statements.
        /// </summary>
        /// <param name="StatementSearchParameter">The Statement search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of Statements
        /// </returns>
        public int GetStatementCount(StatementSearchParameter StatementSearchParameter, string tenantCode)
        {
            int roleCount = 0;
            try
            {
                roleCount = this.StatementRepository.GetStatementCount(StatementSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }

        /// <summary>
        /// This method will call publish Statement method of repository
        /// </summary>
        /// <param name="StatementIdentifier">Statement identifier</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if Statements publish successfully, false otherwise.
        /// </returns>
        public bool PublishStatement(long StatementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementRepository.PublishStatement(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call clone Statement method of repository
        /// </summary>
        /// <param name="StatementIdentifier">Statement identifier</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if Statements clone successfully, false otherwise.
        /// </returns>
        public bool CloneStatement(long StatementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementRepository.CloneStatement(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate Statements.
        /// </summary>
        /// <param name="Statements"></param>
        /// <param name="tenantCode"></param>
        private void IsValidStatements(IList<Statement> Statements, string tenantCode)
        {
            try
            {
                if (Statements?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidStatementException invalidStatementException = new InvalidStatementException(tenantCode);
                Statements.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidStatementException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidStatementException.Data.Count > 0)
                {
                    throw invalidStatementException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate Statement in the list
        /// </summary>
        /// <param name="Statements"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateStatement(IList<Statement> Statements, string tenantCode)
        {
            try
            {
                int isDuplicateStatement = Statements.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateStatement > 0)
                {
                    throw new DuplicateStatementFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}
