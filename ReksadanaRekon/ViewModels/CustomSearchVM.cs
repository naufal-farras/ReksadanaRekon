using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class CustomSearchVM { }

    public class AplikasiSearchVM
    {
        public bool OptionMatch { get; set; }
        public bool Match { get; set; }
        public bool OptionMatchDate { get; set; }
        public DateTime StartMatchDate { get; set; }
        public DateTime EndMatchDate { get; set; }
        public bool OptionSA { get; set; }
        public string SA { get; set; }
        public bool OptionFund { get; set; }
        public string Fund { get; set; }
        public bool OptionMI { get; set; }
        public string MI { get; set; }
        public bool OptionSARef { get; set; }
        public string SARef { get; set ; }
        public bool OptionInvestor { get; set; }
        public string Investor { get; set; }
        public bool OptionNominal { get; set; }
        public int Nominal { get; set; }
    }

    public class FundSearchVM
    {
        public bool OptionMatch { get; set; }
        public bool Match { get; set; }
        public bool OptionMatchDate { get; set; }
        public DateTime StartMatchDate { get; set; }
        public DateTime EndMatchDate { get; set; }
        public bool OptionNoRek { get; set; }
        public string NoRek { get; set; }
        public bool OptionNamaRek { get; set; }
        public string NamaRek { get; set; }
        public bool OptionKeterangan { get; set; }
        public string Keterangan { get; set; }
        public bool OptionJumlah { get; set; }
        public int Jumlah { get; set; }
    }
}