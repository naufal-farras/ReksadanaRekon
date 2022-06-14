using ReksadanaRekon.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class FundReturVM
    {
        public List<DataRetur> allDataRetur { get; set; }
        public List<DataFund> allDataFund { get; set; }
    }
}