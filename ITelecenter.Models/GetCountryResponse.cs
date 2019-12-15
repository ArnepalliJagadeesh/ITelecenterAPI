using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class GetCountryResponse :ServiceResponse
    {
        public List<CountryModel> Countries { get; set; }
    }
    public class CountryModel
    {
        public int CountryID { get; set; }
        public string Country { get; set; }
        public string CountryAbbr { get; set; }
    }

}
