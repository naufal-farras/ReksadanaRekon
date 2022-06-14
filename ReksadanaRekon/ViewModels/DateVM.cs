using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReksadanaRekon.ViewModels
{
    public class DateVM
    {
        public DateTime startdate { get; set; }
        public int RekonType { get; set; }
        public IEnumerable<SelectListItem> RekonTypeList { get; set; }
    }
}