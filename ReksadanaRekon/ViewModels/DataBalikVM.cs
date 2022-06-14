using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.ViewModels
{
   
    public class DataBalikVM
    {
       
        public int FileAwal { get; set; }
        public int DBAwal { get; set; }
        public int DBAkhir { get; set; }
        public int Success { get; set; }
        public int Fails { get; set; }

        public string ERROR { get; set; }
    }
  
}