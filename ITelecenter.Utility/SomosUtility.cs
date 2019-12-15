using ITelecenter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Utility
{
    public class SomosUtility
    {
        public static readonly string uri = $"{ConfigurationManager.AppSettings["APIURL"].ToString()}v2/ip/sec/session/open";
        public static readonly string usrName = ConfigurationManager.AppSettings["SOMOS_UserName"];
        public static readonly string password = ConfigurationManager.AppSettings["SOMOS_PassWord"];
        public static readonly string SOMOS_RespOrgID = ConfigurationManager.AppSettings["SOMOS_RespOrgID"];
        public static readonly string contentType = "application/json";
        public static string GetOAuthToken()
        {
            string OAuthToken = "";
            SomosOAuthToken objOAuthToken = new SomosOAuthToken();
            objOAuthToken = CMSUtility.GetSomosOAuthToken();
            if (objOAuthToken != null)
            {
                if (objOAuthToken.Expired?.ToUpper() == "FALSE")
                {
                    OAuthToken = objOAuthToken.OAuthToken;
                }
                else
                {
                    OAuthToken = GenerateAuthToken();
                    CMSUtility.UpdateSomosOAuthToken(OAuthToken);
                }
            }
            return OAuthToken;
        }

        public static string GenerateAuthToken()
        {
            AuthToken tk = GetSessionStartToken();
            if (tk.sessOverrideKey != null)
            {
                var stroverride = tk.sessOverrideKey;
                AuthToken tk1 = GetSessionStartToken(stroverride);
                return tk1.oauthToken;
            }
            else
            {
                return tk.oauthToken;
            }
        }

        public static AuthToken GetSessionStartToken(String sessoveridekey = "")
        {
            var headers = new List<Tuple<string, string>>();
            String body = "";
            if (sessoveridekey.Length > 0)
                body = "{ \"usrName\" : \"" + usrName + "\", \"password\" : \"" + password + "\", \"withPerm\": false, \"sessOverrideKey\" : \"" + sessoveridekey + "\" }";
            else
                body = "{ \"usrName\" : \"" + usrName + "\", \"password\" : \"" + password + "\", \"withPerm\": false }";
            string apiResult = "";
            bool isapiError = false;
            SomosApiCall(uri, "POST", contentType, headers, body, ref isapiError, ref apiResult, true);
            return JsonConvert.DeserializeObject<AuthToken>(apiResult,
               new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        private static void SomosApiCall(string url, string methodType, string contentType, List<Tuple<string, string>> headers, string methodBody, ref bool isapiError, ref string apiResult, bool readResponse = false)
        {
            //SomosApi.WriteApiErrorLog objSomosApiLog = new SomosApi.WriteApiErrorLog();

            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url.ToString());
            httpWebRequest.Method = methodType;

            if (!string.IsNullOrEmpty(contentType))
            {
                httpWebRequest.ContentType = contentType;
            }

            if (headers != null)
            {
                foreach (var h in headers)
                    httpWebRequest.Headers[h.Item1] = h.Item2;
            }

            if (methodBody != "")
            {
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(methodBody);

                using (Stream datastream = httpWebRequest.GetRequestStream())
                {
                    datastream.Write(bytes, 0, bytes.Length);
                    datastream.Close();
                }
            }

            isapiError = false;
            apiResult = "";

            //DBConnect ObjDBConnect = new DBConnect();

            try
            {
                // Submit Request and Read Response
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted)
                {
                    if (readResponse)
                    {
                        Stream dataStream = httpResponse.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        apiResult = reader.ReadToEnd();
                        reader.Close();
                    }
                }
            }
            catch (System.Net.WebException webException)
            {
                isapiError = true;
                apiResult = webException.Message;
                //ObjDBConnect.WriteTraceLog("Error at SomosApiCall() when using URL: " + url + " , Response : " + webException.Response);
                //objSomosApiLog.WriteExceptionErrorLog("Error at SomosApiCall() when using URL: " + url, webException.Response);
            }
            catch (Exception ex)
            {
                isapiError = true;
                apiResult = ex.Message;
                //ObjDBConnect.WriteTraceLog("Error at SomosApiCall() when using URL: " + url + " , Response : " + ex.Message.ToString());
            }
            finally
            {
                //ObjDBConnect = null;
            }
        }


        public static List<NumberInfo> GetVanityNumbers(string wildcard, string vanityKey, int qty)
        {
            string OAuthToken = GetOAuthToken();
            List<NumberInfo> Numbers = new List<NumberInfo>();
            var uri = ConfigurationManager.AppSettings["APIURL"] + "v2/ip/num/tfn/wildcard";

            var headers = new List<Tuple<string, string>>();
            var stroauth = "Bearer " + OAuthToken;
            headers.Add(new Tuple<string, string>("Authorization", stroauth));
            var body = "{ \"qty\" : \"" + qty + "\", \"wildCardNum\" : \"" + wildcard + "\" }";
            string apiResult = "";
            bool isapiError = false;
            SomosApiCall(uri, "PUT", contentType, headers, body, ref isapiError, ref apiResult, true);
            if (!isapiError)
            {
                if (apiResult != "")
                {
                    JObject ObjJson = JObject.Parse(apiResult);

                    if (ObjJson["numList"] != null)
                    {
                        RandomNumbers objRandomNumbers = new RandomNumbers();

                        // Deserializing json data to object  

                        objRandomNumbers = JsonConvert.DeserializeObject<RandomNumbers>(apiResult);

                        foreach (var number in objRandomNumbers.numList)
                        {
                            NumberInfo num = new NumberInfo();
                            num.phoneNumber = number;
                            num.vanityNumber = formatVanityNumber(number, vanityKey);
                            num.respOrg = SOMOS_RespOrgID;
                            Numbers.Add(num);
                        }

                    }
                }
            }
            return Numbers;

        }

        public static List<NumberInfo> GetRandomNumbers(string npa, string nxx, int qty)
        {
            string OAuthToken = GetOAuthToken();
            List<NumberInfo> Numbers = new List<NumberInfo>();
            var headers = new List<Tuple<string, string>>();
            var stroauth = "Bearer " + OAuthToken;
            headers.Add(new Tuple<string, string>("Authorization", stroauth));
            string uri = "";
            string body = "";
            string apiResult = "";
            bool isapiError = false;
            uri = ConfigurationManager.AppSettings["APIURL"] + "v2/ip/num/tfn/random";

            if (!string.IsNullOrEmpty(nxx))
                body = "{ \"qty\" : \"" + qty + "\", \"npa\" : \"" + npa + "\", \"nxx\": \"" + nxx + "\" }";
            else
                body = "{ \"qty\" : \"" + qty + "\", \"npa\" : \"" + npa + "\" }";
            SomosApiCall(uri, "PUT", contentType, headers, body, ref isapiError, ref apiResult, true);
            if (!isapiError)
            {
                if (apiResult != "")
                {
                    JObject ObjJson = JObject.Parse(apiResult);

                    if (ObjJson["numList"] != null)
                    {
                        RandomNumbers objRandomNumbers = new RandomNumbers();

                        // Deserializing json data to object  
                        objRandomNumbers = JsonConvert.DeserializeObject<RandomNumbers>(apiResult);

                        foreach (var number in objRandomNumbers.numList)
                        {
                            NumberInfo num = new NumberInfo();
                            num.phoneNumber = number;
                            num.vanityNumber = "";
                            num.respOrg = SOMOS_RespOrgID;
                            Numbers.Add(num);
                        }
                    }
                }
            }
            return Numbers;
        }

        private static string formatVanityNumber(string strnumber, string strvanitykey)
        {
            string strnumkey = KeywordSearch(strvanitykey);
            string strvanitynumber = "";

            if (strnumber.Substring(3, 7).IndexOf(strnumkey) > 0)
            {
                strvanitynumber = strnumber.Replace(strnumkey, strvanitykey.ToUpper());
            }
            return strvanitynumber;
        }

        public static string KeywordSearch(string strKeyword)
        {
            try
            {
                strKeyword = strKeyword.Replace("a", "2");
                strKeyword = strKeyword.Replace("A", "2");
                strKeyword = strKeyword.Replace("b", "2");
                strKeyword = strKeyword.Replace("B", "2");
                strKeyword = strKeyword.Replace("c", "2");
                strKeyword = strKeyword.Replace("C", "2");
                strKeyword = strKeyword.Replace("d", "3");
                strKeyword = strKeyword.Replace("D", "3");
                strKeyword = strKeyword.Replace("e", "3");
                strKeyword = strKeyword.Replace("E", "3");
                strKeyword = strKeyword.Replace("f", "3");
                strKeyword = strKeyword.Replace("F", "3");
                strKeyword = strKeyword.Replace("g", "4");
                strKeyword = strKeyword.Replace("G", "4");
                strKeyword = strKeyword.Replace("h", "4");
                strKeyword = strKeyword.Replace("H", "4");
                strKeyword = strKeyword.Replace("i", "4");
                strKeyword = strKeyword.Replace("I", "4");
                strKeyword = strKeyword.Replace("j", "5");
                strKeyword = strKeyword.Replace("J", "5");
                strKeyword = strKeyword.Replace("k", "5");
                strKeyword = strKeyword.Replace("K", "5");
                strKeyword = strKeyword.Replace("l", "5");
                strKeyword = strKeyword.Replace("L", "5");
                strKeyword = strKeyword.Replace("m", "6");
                strKeyword = strKeyword.Replace("M", "6");
                strKeyword = strKeyword.Replace("n", "6");
                strKeyword = strKeyword.Replace("N", "6");
                strKeyword = strKeyword.Replace("o", "6");
                strKeyword = strKeyword.Replace("O", "6");
                strKeyword = strKeyword.Replace("p", "7");
                strKeyword = strKeyword.Replace("P", "7");
                strKeyword = strKeyword.Replace("q", "7");
                strKeyword = strKeyword.Replace("Q", "7");
                strKeyword = strKeyword.Replace("r", "7");
                strKeyword = strKeyword.Replace("R", "7");
                strKeyword = strKeyword.Replace("s", "7");
                strKeyword = strKeyword.Replace("S", "7");
                strKeyword = strKeyword.Replace("t", "8");
                strKeyword = strKeyword.Replace("T", "8");
                strKeyword = strKeyword.Replace("u", "8");
                strKeyword = strKeyword.Replace("U", "8");
                strKeyword = strKeyword.Replace("v", "8");
                strKeyword = strKeyword.Replace("V", "8");
                strKeyword = strKeyword.Replace("w", "9");
                strKeyword = strKeyword.Replace("W", "9");
                strKeyword = strKeyword.Replace("x", "9");
                strKeyword = strKeyword.Replace("X", "9");
                strKeyword = strKeyword.Replace("y", "9");
                strKeyword = strKeyword.Replace("Y", "9");
                strKeyword = strKeyword.Replace("z", "9");
                strKeyword = strKeyword.Replace("Z", "9");
                strKeyword = strKeyword.Replace("-", "");
                strKeyword = strKeyword.Replace("(", "");
                strKeyword = strKeyword.Replace(")", "");

                return strKeyword;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
