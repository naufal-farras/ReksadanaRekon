using ReksadanaRekon.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Trans
{
    public class TrRedAplikasi
    {
        public int Id { get; set; }
        public int TransRedempId { get; set; }
        public TransRedemp TransRedemp { get; set; }
        public int DataRedempId { get; set; }
        public DataRedemp DataRedemp { get; set; }
        public DateTime CreateDate { get; set; }
    }
}