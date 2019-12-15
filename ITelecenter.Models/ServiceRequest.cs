using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class ServiceRequest
    {
        public string webID { get; set; }
        public string cmsOnlineID { get; set; }
        public string packageID { get; set; }
        public string numberType { get; set; }
        public string serviceNumber { get; set; }
    }
}
