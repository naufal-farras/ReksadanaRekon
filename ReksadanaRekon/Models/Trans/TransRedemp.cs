using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Trans
{
    public class TransRedemp : BaseModal
    {
        public int Id { get; set; }
        public Matching Matching { get; set; }
        public int MatchingId { get; set; }
        public string InputerId { get; set; }
        public virtual ApplicationUser Inputer { get; set; }
        public string KeteranganInputer { get; set; }
        public string ApproverId { get; set; }
        public virtual ApplicationUser Approver { get; set; }
        public string KeteranganApprover { get; set; }
    }
}