using ReksadanaRekon.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class RedFundAplikasiVM
    {
        public List<DataRedemp> allDataAplikasi { get; set; }
        public List<DataFundRed> allDataFund { get; set; }
    }
}