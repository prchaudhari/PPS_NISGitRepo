// <copyright file="IStatementRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System.Collections.Generic;
    #endregion

    public interface IStatementRepository
    {
        /// <summary>
        /// This method adds the specified list of statements in statement repository.
        /// </summary>
        /// <param name="statements">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are added successfully, else false.
        /// </returns>
        bool AddStatements(IList<Statement> statements, string tenantCode);

        /// <summary>
        /// This method updates the specified list of statements in statement repository.
        /// </summary>
        /// <param name="statements">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are updated successfully, else false.
        /// </returns>
        bool UpdateStatements(IList<Statement> statements, string tenantCode);

        /// <summary>
        /// This method deletes the specified list of statements from statement repository.
        /// </summary>
        /// <param name="statementIdentifier">The statement identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are deleted successfully, else false.
        /// </returns>
        bool DeleteStatements(long statementIdentifier, string tenantCode);

        /// <summary>
        /// This method gets the specified list of statements from statement repository.
        /// </summary>
        /// <param name="statementSearchParameter">The statement search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of statements
        /// </returns>
        IList<Statement> GetStatements(StatementSearchParameter statementSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get statement count
        /// </summary>
        /// <param name="statementSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Statement count</returns>
        int GetStatementCount(StatementSearchParameter statementSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to publish statement
        /// </summary>
        /// <param name="statementIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool PublishStatement(long statementIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to clone statement
        /// </summary>
        /// <param name="statementIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool CloneStatement(long statementIdentifier, string tenantCode);

        /// <summary>
        /// This method will geenerate preview Statement html string
        /// </summary>
        /// <param name="StatementIdentifier">Statement identifier</param>
        /// <param name="baseURL">API base URL</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns Statements preview html string.
        /// </returns>
        string PreviewStatement(long statementIdentifier, string baseURL, string tenantCode);

        /// <summary>
        /// This method will geenerate HTML format of the Statement
        /// </summary>
        /// <param name="statement">the statment</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns list of statement page content object.
        /// </returns>
        IList<StatementPageContent> GenerateHtmlFormatOfStatement(Statement statement, string tenantCode);

        /// <summary>
        /// This method help to bind preview dara to statement
        /// </summary>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="baseURL"> the base URL of API </param>
        string BindPreviewDataToStatement(Statement statement, IList<StatementPageContent> statementPageContents, string baseURL, string tenantCode);

        /// <summary>
        /// This method help to generate statement for customer
        /// </summary>
        /// <param name="customer"> the customer object </param>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="batchDetails"> the list of batch details records </param>
        /// <param name="baseURL"> the base URL of API </param>
        ScheduleLogDetailRecord GenerateStatements(CustomerMasterRecord customer, Statement statement, IList<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL, string tenantCode, string outputLocation);

        /// <summary>
        /// This method help to bind data to common statement
        /// </summary>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="tenantCode"> the tenant code </param>
        StatementPreviewData BindDataToCommonStatement(Statement statement, IList<StatementPageContent> statementPageContents, string tenantCode);
    }
}
