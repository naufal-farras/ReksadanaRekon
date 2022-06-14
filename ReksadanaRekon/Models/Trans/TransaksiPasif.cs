using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Trans
{
    public class TransaksiPasif : BaseModal
    {
        public int Id { get; set; }
        public Matching Matching { get; set; }
        [ForeignKey("Matching")]
        public int MatchingId { get; set; }
        public string InputerId { get; set; }
        [ForeignKey("InputerId")]
        public virtual ApplicationUser Inputer { get; set; }
        public string KeteranganInputer { get; set; }
        public string ApproverId { get; set; }
        [ForeignKey("ApproverId")]
        public virtual ApplicationUser Approver { get; set; }
        public string KeteranganApprover { get; set; }
        public bool Retur { get; set; } = false;
    }
}