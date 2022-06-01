// <copyright file="SpecialMessage.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class SpecialMessage
    {
        public long Identifier { get; set; }
        public long ParentId { get; set; }
        public long BatchId { get; set; }
        public string Header { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string Message3 { get; set; }
        public string Message4 { get; set; }
        public string Message5 { get; set; }
        public string TenantCode { get; set; }

    }
}
