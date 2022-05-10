// <copyright file="DM_MCAMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class DM_CorporateSaverMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public string AgentContactDetails { get; set; }
        public string AgentClientAddress1 { get; set; }
        public string AgentClientAddress2 { get; set; }
        public string AgentClientAddress3 { get; set; }
        public string AgentClientAddress4 { get; set; }
        public string AgentClientAddress5 { get; set; }
        public string AgentClientAddress6 { get; set; }
        public string VatNo { get; set; }
        public string AgentProfile { get; set; }
        public string BranchCode { get; set; }
        public string CIFNo { get; set; }
        public string ClientCode { get; set; }
        public string RelationshipManager { get; set; }
        public string VATCalculation { get; set; }
        public string ClientVatNo { get; set; }
        public string TaxInvoiceNo { get; set; }
        public string AgentContactPerson { get; set; }
        public string AgentEmailAddress { get; set; }
        public string AgentRegNo { get; set; }
        public string AgentReference { get; set; }
        public string StatementNo { get; set; }
        public string TaxTotalsDescription { get; set; }
        public string TotalInterest { get; set; }
        public string VatOnFee { get; set; }
        public string AgentFeeDeducted { get; set; }
        public string InvestmentDescription { get; set; }
        public string InterestInstruction { get; set; }
        public string InvestmentTotalInterest { get; set; }
        public string AgentFeeStructure1 { get; set; }
        public string AgentFeeStructure2 { get; set; }
        public Nullable<System.DateTime> DateInvested { get; set; }
        public string InvestmentAgentFeeDeducted { get; set; }
        public string InvetsmentVATOnFee { get; set; }
        public string Interest { get; set; }
        public string TenantCode { get; set; }
        public string AgentVATRegNo { get; set; }
        public string AgentFSPLicNo { get; set; }
        public decimal? OverdraftLimit { get; set; }
        public string Currency { get; set; }
        public decimal? FreeBalance { get; set; }
        public DateTime StatementDate { get; set; }
        public string StatementFrequency { get; set; }
        public string AGENT_MESSAGE_ENGLISH { get; set; }
        public string AGENT_MESSAGE_AFRIKAANS { get; set; }
        public bool SHOWMESSAGE { get; set; }
        public string SECTION86_MESSAGE { get; set; }
        public bool MESSAGE_INDICATOR { get; set; }
        public string AGENT_NAME { get; set; }
        public string AGENT_LOGO { get; set; }
        public List<DM_CorporateSaverTransaction> CorporateSaverTransactions { get; set; }
        public List<DM_CorporateSaverTax> CorporateSaverTax { get; set; }
    }

    public class DM_CorporateSaverTransaction
    {
        public long Identifier { get; set; }
        public long Id { get; set; }

        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string PaymentDetails { get; set; }
        public string TransactionDescription { get; set; }
        public string Amount { get; set; }
        public string Rate { get; set; }
        public string CapitalBalance { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_CorporateSaverTax
    {
        public long Identifier { get; set; }
        public long Id { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public Nullable<decimal> CapitalBalance { get; set; }
        public string TenantCode { get; set; }
        public string InvestType { get; set; }
        public string InterestIntruction { get; set; }
        public Nullable<System.DateTime> DateInvested { get; set; }
        public string CapitalMstr { get; set; }
        public string AgentFeeDeducted { get; set; }
        public string InterestMstr0 { get; set; }
        public string VatOnFeeMstr { get; set; }
        public string AGENT_FEE_STRUCTURE_1 { get; set; }
        public string AGENT_FEE_STRUCTURE_2 { get; set; }
        public string InterestMstr { get; set; }
        public string TotalCapitalMstr { get; set; }
        public string TotalInterestMstr { get; set; }
        public string TotalAgentFeeMstr { get; set; }
        public string VatOnFeeMstr0 { get; set; }
        public string InterestAgentFeeMstr { get; set; }
        public string InterestDescription { get; set; }

    }

}
