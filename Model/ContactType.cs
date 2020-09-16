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
    public enum ContactType
    {
        [Description("None")]
        None = 0,

        [Description("Primary")]
        Primary = 1,

        [Description("Secondary")]
        Secondary = 2,

        [Description("Billing")]
        Billing = 3
    }
}
