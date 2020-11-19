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
        bool RunArchivalProcess(Client client, int ParallelThreadCount, int MinimumArchivalPeriodDays, string pdfStatementFilepath, string htmlStatementFilepath, TenantConfiguration tenantConfiguration, string tenantCode);
    }
}
