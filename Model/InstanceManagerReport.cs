// <copyright file="Asset.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References

    using System;
    using System.ComponentModel;

    #endregion

    public class InstanceManagerReport
    {
        #region Private members

        /// <summary>
        /// The totalGroup.
        /// </summary>
        private long totalGroup;

        /// <summary>
        /// The totalTenant.
        /// </summary>
        private long totalTenant;

        /// <summary>
        /// The publishedStatementByGroup.
        /// </summary>
        private PiChartGraphData publishedStatementByGroup;

        /// <summary>
        /// The usersByGroup.
        /// </summary>
        private GraphChartData usersByGroup;

        /// <summary>
        /// The statementByGroup.
        /// </summary>
        private GraphChartData statementByGroup;

        #endregion

        #region Public members
        /// <summary>
        /// Gets or sets totalGroup.
        /// </summary>
        public long TotalGroup
        {
            get
            {
                return this.totalGroup;
            }

            set
            {
                this.totalGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets totalGroup.
        /// </summary>
        public long TotalTenant
        {
            get
            {
                return this.totalTenant;
            }

            set
            {
                this.totalTenant = value;
            }
        }

        /// <summary>
        /// Gets or sets publishedStatementByGroup.
        /// </summary>
        public PiChartGraphData PublishedStatementByGroup
        {
            get
            {
                return this.publishedStatementByGroup;
            }

            set
            {
                this.publishedStatementByGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets publishedStatementByGroup.
        /// </summary>
        public GraphChartData UsersByGroup
        {
            get
            {
                return this.usersByGroup;
            }

            set
            {
                this.usersByGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets publishedStatementByGroup.
        /// </summary>
        public GraphChartData StatementByGroup
        {
            get
            {
                return this.statementByGroup;
            }

            set
            {
                this.statementByGroup = value;
            }
        } 
        #endregion

    }

}
