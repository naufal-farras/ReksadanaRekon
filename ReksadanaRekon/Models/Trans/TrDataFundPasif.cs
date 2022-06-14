using ReksadanaRekon.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Trans
{
    public class TrDataFundPasif
    {
        public int Id { get; set; }
        public int TransaksiId { get; set; }
        [ForeignKey("TransaksiId")]
        public TransaksiPasif Transaksi { get; set; }
        public int DataFundId { get; set; }
        [ForeignKey("DataFundId")]
        public DataFundPasif DataFund { get; set; }
        public DateTime CreateDate { get; set; }
    }
}