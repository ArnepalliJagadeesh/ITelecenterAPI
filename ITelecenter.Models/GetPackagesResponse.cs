using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class GetPackagesResponse : ServiceResponse
    {
        public List<Package> Packages { get; set; }
    }

}

