using ReksadanaRekon.Models.Trans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class TransRedFundVM
    {
        public List<TrRedAplikasi> allDataAplikasi { get; set; }
        public List<TrRedFund> allDataFund { get; set; }
    }
}