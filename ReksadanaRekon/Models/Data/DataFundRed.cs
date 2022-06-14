using ReksadanaRekon.Models.Core;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Data
{
    public class DataFundRed : BaseModal
    {
        public int Id { get; set; }
        public DateTime Tanggal { get; set; }
        public string Keterangan { get; set; }
        public string KeteranganDua { get; set; }
        public long Debit { get; set; }
        public long Credit { get; set; }
        public long? Saldo { get; set; }
        public Matching Matching { get; set; }
        public int MatchingId { get; set; }
        public Rekening Rekening { get; set; }
        public int RekeningId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string KeteranganUser { get; set; }

        //public DataFundRed()
        //{
        //    CreateDate = DateTime.Now;
        //    IsDelete = false;
        //}
    }
}