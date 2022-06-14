using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Master
{
    public class Fund : BaseModal
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Nama { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}