// <copyright file="ArchivalProcessRawData.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class ArchivalProcessRawData
    {
        public Statement Statement { get; set; }
        public long CustomerId { get; set; }
        public IList<StatementPageContent> StatementPageContents { get; set; }
        public TenantConfiguration TenantConfiguration { get; set; }
        public string PdfStatementFilepath { get; set; }
        public string HtmlStatementFilepath { get; set; }
        public Client Client { get; set; }
        public ScheduleLogArchive ScheduleLogArchive { get; set; }
        public ScheduleLog ScheduleLog { get; set; }
        public RenderEngine RenderEngine { get; set; }
        public Schedule Schedule { get; set; }
        public BatchMaster BatchMaster { get; set; }
        public IList<BatchDetail> BatchDetails { get; set; }
    }
}
