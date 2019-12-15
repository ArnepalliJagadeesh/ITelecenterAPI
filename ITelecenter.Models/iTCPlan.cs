using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class iTCPlan
    {
        public string serviceNumber { get; set; }
        public string numberType { get; set; }
        public string packageID { get; set; }
        public string packageName { get; set; }
        public string baseserviceTCPlan { get; set; }
        public string serviceFee { get; set; }
        public string setupFee { get; set; }
    }
}
