using ITelecenter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Utility
{
    public class IntelePeerUtility
    {
        public static readonly string APIUserName = ConfigurationManager.AppSettings["INTELEPEER_API_USERID"].ToString();
        public static readonly string APIPassWord = ConfigurationManager.AppSettings["INTELEPEER_API_PASSWORD"].ToString();
        public static readonly string APIEndPoint = ConfigurationManager.AppSettings["INTELEPEER_BASE_URL"].ToString();
        public static readonly string INTELEPPER_RESPORGID = ConfigurationManager.AppSettings["INTELEPPER_RESPORGID"];
        public static string GenerateAuthToken(ref bool isError, ref string result)
        {
            string AUTH_TOKEN = "";
            try
            {
                // Create Content for Request
                string json = getAuthJsonBody();

                string url = string.Format("{0}{1}", APIEndPoint, Constants.IntelePeerInfo.url_AuthToken);

                isError = false;
                result = "";

                POST_IntelePeerServiceAPI(url, "", json, ref isError, ref result);

                if (isError == false & result.Length > 3)
                {
                    AuthorizationResponse objAuthRes = JsonConvert.DeserializeObject<AuthorizationResponse>(result);
                    AUTH_TOKEN = objAuthRes.token;
                }
            }
            catch (Exception ex)
            {
                //DBConnect ObjDBConnect = new DBConnect();
                //ObjDBConnect.WriteTraceLog("Exception Occured in GenerateAuthToken method -   : " + ex.ToString());
                //throw ex;
            }
            return AUTH_TOKEN;
        }

        public static string getAuthJsonBody()
        {
            // Create Content for Request
            authKeys ak = new authKeys();
            ak.username = APIUserName;
            ak.password = APIPassWord;
            string json = JsonConvert.SerializeObject(ak);
            return json;
        }

        private static void POST_IntelePeerServiceAPI(string url, string token, string serializedJsonBody, ref bool isError, ref string result)
        {
            // IntelePeerAPI.WriteApiErrorLog objIntelepeerLogs = new IntelePeerAPI.WriteApiErrorLog();

            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url.ToString());

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            if (token.Length > 5)
                httpWebRequest.Headers["Authorization"] = token;

            httpWebRequest.ContentLength = serializedJsonBody.Length;

            isError = false;
            result = "";

            try
            {
                // Writing the json body to the Stream
                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(httpWebRequest.GetRequestStream());
                streamWriter.Write(serializedJsonBody);
                streamWriter.Close();

                // Submit Request and Read Response
                System.Net.WebResponse httpResponse = httpWebRequest.GetResponse();

                System.IO.StreamReader streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream());
                result = streamReader.ReadToEnd();
                streamReader.Close();
            }
            catch (System.Net.WebException webException)
            {
                isError = true;
                result = webException.Message;

                //objIntelepeerLogs.WriteExceptionErrorLog("Error at POST_IntelePeerServiceAPI() when using URL: " + url, webException.Response);
            }

            catch (Exception exception)
            {
                isError = true;
                result = exception.Message;
                //objIntelepeerLogs.WriteIntelePeerLog("General Error at POST_IntelePeerServiceAPI(), result: " + result);
            }
        }


        public static List<NumberInfo> TryToFetchAndAppendIntelePeerDIDNumbers(string strStateAbbrv, string strCityName, string strNpa)
        {
            List<NumberInfo> Numbers = new List<NumberInfo>();
            try
            {
                //objIntLog.WriteIntelePeerFlowLog("   >>> TryToFechNumbers(...)...TryToFetchAuthToken(...)");

                bool isTokenError = false;
                string strAuthToken = string.Empty;

                bool isError = false;
                string result = "";

                if (isTokenError == false)
                {
                    //IntelePeerAPI.clsIntelePeer objIntPeer = new IntelePeerAPI.clsIntelePeer();
                    string AUTH_TOKEN = GenerateAuthToken(ref isError, ref result);
                    strAuthToken = AUTH_TOKEN;

                    if (isError == false)
                    {
                        FetchDIDNumbers(strAuthToken, strStateAbbrv, strCityName, strNpa, ref isError, ref result);
                        if (isError == false)
                        {
                            var numbersList = new List<clsTollFreeInventory>();
                            numbersList =  JsonConvert.DeserializeObject<List<clsTollFreeInventory>>(result);

                            foreach (var inventory in numbersList)
                            {
                                NumberInfo num = new NumberInfo();
                                num.phoneNumber = inventory.number;
                                num.vanityNumber = "";
                                num.respOrg = INTELEPPER_RESPORGID;
                                Numbers.Add(num);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                //WriteTraceLog("Exception occured in TryToFetchAndAppendIntelePeerDIDNumbers metod : " + Ex.ToString());
                //throw Ex;
            }
            return Numbers;

        }

        public static void FetchDIDNumbers(string token, string StateAbbrv, string CityName, string NPA, ref bool isError, ref string result)
        {
            QueryStringBuilder queryVariables = new QueryStringBuilder();
            if (!string.IsNullOrEmpty(StateAbbrv.Trim()))
                queryVariables.AddItem("state", StateAbbrv);

            if (!string.IsNullOrEmpty(NPA.Trim()))
            {
                if (System.Convert.ToInt32(NPA) > 0)
                    queryVariables.AddItem("npa", NPA);
            }

            if (!string.IsNullOrEmpty(CityName.Trim()))
                queryVariables.AddItem("city", CityName);

            string url = string.Format("{0}{1}?{2}", APIEndPoint,Constants.IntelePeerInfo.url_InventorySearch_Local_DID, queryVariables.ToString());

            GET_IntelePeerServiceAPI(url, token, ref isError, ref result);
        }

        private static void GET_IntelePeerServiceAPI(string url, string token, ref bool isError, ref string result)
        {
          //  IntelePeerAPI.WriteApiErrorLog objIntelepeerLogs = new IntelePeerAPI.WriteApiErrorLog();

            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url.ToString());
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers["Authorization"] = token;

            isError = false;
            result = "";

            try
            {
                // Submit Request and Read Response
                System.Net.WebResponse httpResponse = httpWebRequest.GetResponse();

                System.IO.StreamReader streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream());
                result = streamReader.ReadToEnd();
                streamReader.Close();
            }
            catch (System.Net.WebException webException)
            {
                isError = true;
                result = webException.Message;
                //DBConnect ObjDBConnect = new DBConnect();
                //ObjDBConnect.WriteTraceLog("Error at GET_IntelePeerServiceAPI() when using URL: " + url + " , Response : " + webException.Response);
                //objIntelepeerLogs.WriteExceptionErrorLog("Error at GET_IntelePeerServiceAPI() when using URL: " + url, webException.Response);
            }
        }
    }
}
