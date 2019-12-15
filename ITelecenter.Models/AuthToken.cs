using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
   public class AuthToken
    {
        //oauthToken, refreshToken, clientKey, clientSecret, sessOverrideKey, commonAuthId
        public string oauthToken { get; set; }
        public string refreshToken { get; set; }
        public string clientKey { get; set; }
        public string clientSecret { get; set; }
        public string sessOverrideKey { get; set; }
        public string commonAuthId { get; set; }
    }
}
