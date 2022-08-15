using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Data
{
    public class DataRetur : BaseModal
    {
        public int Id { get; set; }
        public DateTime TransDate { get; set; }
        public string NoRekening { get; set; }        
        public string NamaNasabah { get; set; }
        public string RekeningNasabah { get; set; }
        //public Bank Bank { get; set; }
        //public int BankId { get; set; }
        public string NamaBank{ get; set; }
        public long Nominal { get; set; }
        public string KeteranganRetur { get; set; }
        public Matching Matching { get; set; }
        public int MatchingId { get; set; }
        public SA SA { get; set; }
        public int SAId { get; set; }
        public Fund Fund { get; set; }
        public int FundId { get; set; }
        public MI MI { get; set; }
        public int MIId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string KeteranganUser { get; set; }
        public DateTime PaymentDate { get; set; }
        public string IFUA { get; set; }
        public string NoJurnal { get; set; }
        public string IFUAName { get; set; }
        public string SARefrence { get; set; }
    }
}