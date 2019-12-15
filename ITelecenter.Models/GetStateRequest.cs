using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class GetStateRequest :ServiceRequest
    {
        public string countryAbbr { get; set; }
        public int CountryID { get; set; }
    }
    public class GetStateResponse : ServiceResponse
    {
        public List<StateModel> States { get; set; }
    }
    public class StateModel
    {
        public int stateID { get; set; }
        public string State { get; set; }
    }
}
