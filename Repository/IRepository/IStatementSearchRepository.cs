// <copyright file="IStatementSearchRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System.Collections.Generic;
    #endregion

    public interface IStatementSearchRepository
    {
        /// <summary>
        /// This method adds the specified list of statements in statement repository.
        /// </summary>
        /// <param name="statements">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are added successfully, else false.
        /// </returns>
        bool AddStatementSearchs(IList<StatementSearch> statements, string tenantCode);

        /// <summary>
        /// This method gets the specified list of statements from statement repository.
        /// </summary>
        /// <param name="statementSearchParameter">The statement search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of statements
        /// </returns>
        IList<StatementSearch> GetStatementSearchs(StatementSearchSearchParameter statementSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get statement count
        /// </summary>
        /// <param name="statementSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>StatementSearch count</returns>
        int GetStatementSearchCount(StatementSearchSearchParameter statementSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to generate html statement for export to pdf
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>output location of html statmeent file</returns>
        string GenerateStatement(long identifier, string tenantCode, Client client);

        /// <summary>
        /// This method help to generate statement for customer
        /// </summary>
        /// <param name="customer"> the customer object </param>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="batchDetails"> the list of batch details records </param>
        /// <param name="outputLocation"> the output file path </param>
        /// <param name="client"> the client object </param>
        /// <param name="tenantConfiguration"> the tenant configuration object </param>
        /// <param name="tenantCode"> the tenant code </param>
        string GenerateHtmlStatementForPdfGeneration(CustomerMasterRecord customer, Statement statement, IList<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string tenantCode, string outputLocation, Client client, TenantConfiguration tenantConfiguration);
    }
}
