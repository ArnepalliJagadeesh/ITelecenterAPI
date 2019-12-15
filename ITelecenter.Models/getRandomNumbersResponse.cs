using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class getRandomNumbersResponse: ServiceResponse
    {
        public List<NumberInfo> Numbers { get; set; }
    }
}
