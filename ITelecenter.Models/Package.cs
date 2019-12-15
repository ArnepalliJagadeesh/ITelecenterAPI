using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class Package
    {
        public string nmcompanyID { get; set; }
        public string PackageID { get; set; }
        public string PackageName { get; set; }
        public string SetUpFee { get; set; }
        public string ServiceFee { get; set; }
        public string usperMint { get; set; }
        public string inboundMinutes { get; set; }
        public string freeTrial { get; set; }
        public string maxPhoneNumbers { get; set; }
        public string maxExtensions { get; set; }
        public string trialMaxMinutes { get; set; }

        public string PackageBaseServiceTCPlanID { get; set; }
    }
}
