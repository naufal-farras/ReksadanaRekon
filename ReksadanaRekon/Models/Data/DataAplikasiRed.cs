using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Data
{
    public class DataAplikasiRed : BaseModal
    {
        public int Id { get; set; }
        public string CIF { get; set; }
        public string CIF_APERD { get; set; }
        public string AccNum { get; set; }
        public string HolderName { get; set; }
        public string RedempNo { get; set; }
        public DateTime TransDate { get; set; }
        public long PayAmount { get; set; }
        public long UnitRedeem { get; set; }
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
    }
}