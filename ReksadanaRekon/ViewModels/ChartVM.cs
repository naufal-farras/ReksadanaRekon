using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class ChartVM
    {
        public string Label { get; set; }
        public IEnumerable<DatasetVM> DatasetVM { get; set; }
    }

    public class DatasetVM
    {
        public string Labels { get; set; }
        public decimal Total { get; set; }
    }
}