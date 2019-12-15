using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class clsTollFreeInventory
    {
        public int countryCode { get; set; }
        public int countryID { get; set; }
        public string countryName { get; set; }
        public string e164 { get; set; }
        public string number { get; set; }
    }
}
