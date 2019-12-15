using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class getRandomNumbersRequest:ServiceRequest
    {
        public string npa { get; set; }
        public string nxx { get; set; }

        public string vanityKey { get; set; }
        public string stateAbbr { get; set; }
        public string city { get; set; }
        public Int32 qty { get; set; }
      
    }
}
