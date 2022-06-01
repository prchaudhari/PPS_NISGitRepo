// <copyright file="DM_PersonalLoanArrears.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class DM_PersonalLoanArrears
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public string Arrears_120 { get; set; }
        public string Arrears_90 { get; set; }
        public string Arrears_60 { get; set; }
        public string Arrears_30 { get; set; }
        public string Arrears_0 { get; set; }
        public string TenantCode { get; set; }
    }
}
