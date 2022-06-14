using ReksadanaRekon.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Trans
{
    public class TrDataAplikasi
    {
        public int Id { get; set; }
        public int TransaksiId { get; set; }
        [ForeignKey("TransaksiId")]
        public Transaksi Transaksi { get; set; }
        public int DataAplikasiId { get; set; }
        [ForeignKey("DataAplikasiId")]
        public DataAplikasi DataAplikasi { get; set; }
        public DateTime CreateDate { get; set; }
    }
}