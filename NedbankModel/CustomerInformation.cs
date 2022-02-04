// <copyright file="CustomerInformation.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace NedbankModel
{
    /// <summary>
    /// The CustomerInformation model
    /// </summary>
    public class CustomerInformation
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        /// <value>
        /// The batch identifier.
        /// </value>
        public long BatchId { get; set; }
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        public long CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the investor identifier.
        /// </summary>
        /// <value>
        /// The investor identifier.
        /// </value>
        public long InvestorId { get; set; }
        /// <summary>
        /// Gets or sets the branch identifier.
        /// </summary>
        /// <value>
        /// The branch identifier.
        /// </value>
        public long BranchId { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the name of the sur.
        /// </summary>
        /// <value>
        /// The name of the sur.
        /// </value>
        public string SurName { get; set; }
        /// <summary>
        /// Gets or sets the address line0.
        /// </summary>
        /// <value>
        /// The address line0.
        /// </value>
        public string AddressLine0 { get; set; }
        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        /// <value>
        /// The address line1.
        /// </value>
        public string AddressLine1 { get; set; }
        /// <summary>
        /// Gets or sets the address line2.
        /// </summary>
        /// <value>
        /// The address line2.
        /// </value>
        public string AddressLine2 { get; set; }
        /// <summary>
        /// Gets or sets the address line3.
        /// </summary>
        /// <value>
        /// The address line3.
        /// </value>
        public string AddressLine3 { get; set; }
        /// <summary>
        /// Gets or sets the address line4.
        /// </summary>
        /// <value>
        /// The address line4.
        /// </value>
        public string AddressLine4 { get; set; }
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Gets or sets the mask cell no.
        /// </summary>
        /// <value>
        /// The mask cell no.
        /// </value>
        public string MaskCellNo { get; set; }
        /// <summary>
        /// Gets or sets the barcode.
        /// </summary>
        /// <value>
        /// The barcode.
        /// </value>
        public string Barcode { get; set; }
        /// <summary>
        /// Gets or sets the tenant code.
        /// </summary>
        /// <value>
        /// The tenant code.
        /// </value>
        public string TenantCode { get; set; }
    }
}
