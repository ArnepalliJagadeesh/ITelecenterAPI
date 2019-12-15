using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class CreateVisitorInformationRequest :ServiceRequest
    {
        public string ipAddress { get; set; }
        public int voiceTranscription { get; set; }
        public string responseType { get; set; }
    }

    public class CreateVisitorInformationResponse :ServiceResponse
    {
        public string CMSOnlineID { get; set; }
    }
}
