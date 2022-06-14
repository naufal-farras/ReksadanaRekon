using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Data
{
    public class DataRedemp : BaseModal
    {
        public int Id { get; set; }
        public DateTime TransDate { get; set; }
        public string ReferenceNo { get; set; }
        public long Nominal { get; set; }
        public string Nasabah { get; set; }
        public string KeteranganRetur { get; set; }
        public Matching Matching { get; set; }
        public int MatchingId { get; set; }
        public SA SA { get; set; }
        public int? SAId { get; set; }
        public Fund Fund { get; set; }
        public int FundId { get; set; }
        public MI MI { get; set; }
        public int MIId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string KeteranganUser { get; set; }
        public bool ByInput { get; set; }
    }
}