using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
    public class ResultUpload
    {
        public string namafile { get; set; }
        public string status { get; set; }
        public string warna { get; set; }
        public int success { get; set; }
        public int fails { get; set; }
        public string error { get; set; }
    }
}