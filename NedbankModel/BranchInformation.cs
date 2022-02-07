// <copyright file="BranchInformation.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace NedbankModel
{
    /// <summary>
    /// The BranchInformation model
    /// </summary>
    public class BranchInformation
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The branch id
        /// </summary>
        public long BranchId { get; set; }

        /// <summary>
        /// Then branch name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The address line 0
        /// </summary>
        public string AddressLine0 { get; set; }

        /// <summary>
        /// The address line 1
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// The address line 2
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// The address line 3
        /// </summary>
        public string AddressLine3 { get; set; }

        /// <summary>
        /// The address line 4
        /// </summary>
        public string AddressLine4 { get; set; }

        /// <summary>
        /// The vat registration number
        /// </summary>
        public string VatRegNo { get; set; }

        /// <summary>
        /// The contact number 
        /// </summary>
        public string ContactNo { get; set; }

        /// <summary>
        /// The tenant code
        /// </summary>
        public string TenantCode { get; set; }
    }
}
