using ITelecenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Utility
{
    public class NumberSearchUtility
    {
        public static getRandomNumbersResponse GetRandomNumbers(getRandomNumbersRequest request)
        {
            getRandomNumbersResponse response = new getRandomNumbersResponse();
            try
            {
                //TODO: Get Data From Data Base
            }
            catch (Exception ex)
            {
                //TODO: FogBug Logging
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }
    }
}
