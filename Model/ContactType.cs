// <copyright file="ContactType.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2016 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------


namespace nIS
{

    #region References

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This enum indicates the types of contacts.
    /// </summary>
    public class ContactType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string TenantCode { get; set; }

    }
}
