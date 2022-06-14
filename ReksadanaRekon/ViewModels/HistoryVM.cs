using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class HistoryVM { }
    public class HistoryAppVM
    {
        public int TransaksiId { get; set; }
        public DateTime TanggalUpload { get; set; }
        public DateTime? TanggalMatch { get; set; }
        public DateTime TanggalTransaksi { get; set; }
        public string SA { get; set; }
        public string Fund { get; set; }
        public string MI { get; set; }
        public string SAReference { get; set; }
        public string InvestorFundUnitName { get; set; }
        public long AmountNominal { get; set; }
        public int MatchingId { get; set; }
        public string MatchingWarna { get; set; }
        public string MatchingNama { get; set; }

        //Red & Swi
        public string HolderName { get; set; }
        public long PayAmount { get; set; }
    }
    public class HistoryFundVM
    {
        public int TransaksiId { get; set; }
        public DateTime TanggalUpload { get; set; }
        public DateTime? TanggalMatch { get; set; }
        public DateTime TanggalTransaksi { get; set; }
        public string NoRek { get; set; }
        public string NamaRek { get; set; }
        public string Keterangan { get; set; }
        public long Jumlah { get; set; }
        public int MatchingId { get; set; }
        public string MatchingWarna { get; set; }
        public string MatchingNama { get; set; }

        //Red & Swi
        public long Credit { get; set; }
        public long Debit { get; set; }
    }
}