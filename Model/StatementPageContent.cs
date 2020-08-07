// <copyright file="StatementPageContent.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represents statement page content model.
    /// </summary>
    public class StatementPageContent
    {
        public int Id { get; set; }
        public long PageId { get; set; }
        public long PageTypeId { get; set; }
        public string DisplayName { get; set; }
        public string HtmlContent { get; set; } 
        public string TabClassName { get; set; }
        public string PageHeaderContent { get; set; }
        public string PageFooterContent { get; set; }
    }
}
