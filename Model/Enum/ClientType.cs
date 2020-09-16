// <copyright file="ClientType.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represents ClientType enum
    /// </summary>
    public enum ClientType
    {
        [Description("Asset")]
        Asset = 1,

        [Description("Service")]
        Service = 2,

        [Description("Both")]
        Both = 3
    }
}
