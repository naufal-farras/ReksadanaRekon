using ReksadanaRekon.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Trans
{
    public class TrDataAplikasiPasif
    {
        public int Id { get; set; }
        public int TransaksiId { get; set; }
        [ForeignKey("TransaksiId")]
        public TransaksiPasif Transaksi { get; set; }
        public int DataAplikasiId { get; set; }
        [ForeignKey("DataAplikasiId")]
        public DataAplikasiPasif DataAplikasi { get; set; }
        public DateTime CreateDate { get; set; }
    }
}