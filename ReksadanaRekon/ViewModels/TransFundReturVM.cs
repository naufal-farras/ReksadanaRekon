using ReksadanaRekon.Models.Trans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class TransFundReturVM
    {
        public List<TrDataRetur> allDataAplikasi { get; set; }
        public List<TrDataFund> allDataFund { get; set; }
    }
}