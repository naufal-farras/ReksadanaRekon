using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Master
{
    public class Rekening : BaseModal
    {
        public int Id { get; set; }
        public string NoRek { get; set; }
        public string NamaRek { get; set; }
        public SA SA { get; set; }
        [ForeignKey("SA")]
        public Nullable<int> SAId { get; set; }
        public Fund Fund { get; set; }
        [ForeignKey("Fund")]
        public Nullable<int> FundId { get; set; }
        public MI MI { get; set; }
        [ForeignKey("MI")]
        public Nullable<int> MIId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}