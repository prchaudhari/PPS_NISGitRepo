// <copyright file="GenerateStatementRawData.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace nIS
{
    public class GenerateStatementRawData
    {
        public CustomerMaster Customer { get; set; }
        public Statement Statement { get; set; }
        public ScheduleLog ScheduleLog { get; set; }
        public ScheduleLogDetail ScheduleLogDetail { get; set; }
        public IList<StatementPageContent> StatementPageContents { get; set; }
        public BatchMaster Batch { get; set; }
        public IList<BatchDetail> BatchDetails { get; set; }
        public string BaseURL { get; set; }
        public long CustomerCount { get; set; }
        public string OutputLocation { get; set; }
        public TenantConfiguration TenantConfiguration { get; set; }
        public Client Client { get; set; }
        public IList<TenantEntity> TenantEntities { get; set; }
        public RenderEngine RenderEngine { get; set; }
    }
}
