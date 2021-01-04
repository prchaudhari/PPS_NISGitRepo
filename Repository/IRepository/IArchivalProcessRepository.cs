// <copyright file="IArchivalProcessRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References
    using System.Collections.Generic;
    #endregion

    public interface IArchivalProcessRepository
    {

        /// <summary>
        /// This method to convert HTML statements into the PDF statement files and archieve related to log and metadata.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ParallelThreadCount"></param>
        /// <param name="MinimumArchivalPeriodDays"></param>
        /// <param name="pdfStatementFilepath"></param>
        /// <param name="htmlStatementFilepath"></param>
        /// <param name="tenantConfiguration"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the archive process runs successfully, false otherwise
        /// </returns>
        bool RunArchivalProcess(Client client, int ParallelThreadCount, int MinimumArchivalPeriodDays, string pdfStatementFilepath, string htmlStatementFilepath, TenantConfiguration tenantConfiguration, string tenantCode);

        /// <summary>
        /// This method adds the specified list of schedule log archive in the repository.
        /// </summary>
        /// <param name="scheduleLogArchives"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log archive values are added successfully, false otherwise
        /// </returns>
        bool SaveScheduleLogArchieve(IList<ScheduleLogArchive> scheduleLogArchives, string tenantCode);

        /// <summary>
        /// This method gets the specified list of schedule log archive records from repository.
        /// </summary>
        /// <param name="ScheduleId">The schedule identifier</param>
        /// <param name="BatchId">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of schedule log archive records
        /// </returns>
        IList<ScheduleLogArchive> GetScheduleLogArchives(long ScheduleId, long BatchId, string tenantCode);

        /// <summary>
        /// This method adds the specified list of schedule log detail archieve in the repository.
        /// </summary>
        /// <param name="scheduleLogDetailArchieves"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log details archieve values are added successfully, false otherwise
        /// </returns>
        bool SaveScheduleLogDetailsArchieve(IList<ScheduleLogDetailArchieve> scheduleLogDetailArchieves, string tenantCode);

        /// <summary>
        /// This method adds the specified list of statement metadata archieve in the repository.
        /// </summary>
        /// <param name="statementMetadataArchives"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the statement metadata archieve values are added successfully, false otherwise
        /// </returns>
        bool SaveStatementMetadataArchieve(IList<StatementMetadataArchive> statementMetadataArchives, string tenantCode);
    }
}
