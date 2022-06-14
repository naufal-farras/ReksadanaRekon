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
    public class DataFund : BaseModal
    {
        public int Id { get; set; }
        public string CCY { get; set; }
        public DateTime Tanggal { get; set; }
        public string Keterangan { get; set; }
        public string KeteranganDua { get; set; }
        public Int64 Jumlah { get; set; }
        public Int64 Saldo { get; set; }
        public Matching Matching { get; set; }
        [ForeignKey("Matching")]
        public int MatchingId { get; set; }
        public Rekening Rekening { get; set; }
        [ForeignKey("Rekening")]
        public int RekeningId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string KeteranganUser { get; set; }
    }
}