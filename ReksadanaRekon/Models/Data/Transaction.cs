using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ReksadanaRekon.Models.Data;

namespace ReksadanaRekon.Models.Data
{
    public class Transaction : BaseModal
    {
        public int Id { get; set; }
        public DataFund DataFund { get; set; }
        [ForeignKey("DataFund")]
        public Nullable<int> DataFundId { get; set; }
        public DataAplikasi DataAplikasi { get; set; }
        [ForeignKey("DataAplikasi")]
        public Nullable<int> DataAplikasiId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string KeteranganUser { get; set; }
    }
}