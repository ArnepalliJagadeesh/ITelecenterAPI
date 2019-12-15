using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class authKeys
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class AuthorizationResponse
    {
        public string token;
    }
}
