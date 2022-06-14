using ReksadanaRekon.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReksadanaRekon.Models.Master
{
    public class Bank : BaseModal
    {
        public int Id { get; set; }
        public string Nama { get; set; }
    }
}