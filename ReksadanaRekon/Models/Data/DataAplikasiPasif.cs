
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Data
{
    public class DataAplikasiPasif : BaseModal
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string ReferenceNo { get; set; }
        public string Status { get; set; }
        public string IMFeeAmendment { get; set; }
        public string IMPaymentDate { get; set; }
        public string SACode { get; set; }
        public string SAName { get; set; }
        public string InvestorFundUnitNo { get; set; }
        public string InvestorFundUnitName { get; set; }
        public string SID { get; set; }
        public string FundCode { get; set; }
        public string FundName { get; set; }
        public string IMCode { get; set; }
        public string IMName { get; set; }
        public string CBCode { get; set; }
        public string CBName { get; set; }
        public string FundCCY { get; set; }
        public Int64 AmountNominal { get; set; }
        public string AmountUnit { get; set; }
        public string AmountAll { get; set; }
        public string FeeNominal { get; set; }
        public string FeeUnit { get; set; }
        public string FeePercent { get; set; }
        public string TransferPath { get; set; }
        public string REDMSequentialCode { get; set; }
        public string REDMBICCode { get; set; }
        public string REDMBIMemberCode { get; set; }
        public string REDMBankName { get; set; }
        public string REDMNo { get; set; }
        public string REDMName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string TransferType { get; set; }
        public DateTime? InputDate { get; set; }
        public string UploadReference { get; set; }
        public string SAReference { get; set; }
        public string IMStatus { get; set; }
        public string CBStatus { get; set; }
        public string CBCompletionStatus { get; set; }
        public Matching Matching { get; set; }
        [ForeignKey("Matching")]
        public int MatchingId { get; set; }
        public SA SA { get; set; }
        [ForeignKey("SA")]
        public int SAId { get; set; }
        public Fund Fund { get; set; }
        [ForeignKey("Fund")]
        public int FundId { get; set; }
        public MI MI { get; set; }
        [ForeignKey("MI")]
        public int MIId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string KeteranganUser { get; set; }
    }
}