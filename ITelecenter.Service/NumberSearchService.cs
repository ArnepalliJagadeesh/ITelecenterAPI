using ITelecenter.Models;
using ITelecenter.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITelecenter.Service
{
    public class NumberSearchService
    {
        public static readonly string NumberFetchType = ConfigurationManager.AppSettings["NUMBERS_FETCH_AND_SHOW_MODEL"].ToString();
        public static readonly string DefaultWebID = ConfigurationManager.AppSettings["DefaultWebID"].ToString();
        public static readonly int WebSiteID = Convert.ToInt32(ConfigurationManager.AppSettings["WebSiteID"]);
        public static getRandomNumbersResponse GetRandomNumbers(getRandomNumbersRequest request)
        {
            getRandomNumbersResponse response = new getRandomNumbersResponse();
            try
            {
                if (ValidateRandomNumbersRequest(request, response))
                {
                    GetRandomNumbersList(request, response);
                    response.Status = Constants.ServiceResponses.Success;
                    response.ResponseMessage = Constants.ServiceResponses.SuccessResponseMessage;
                }
            }
            catch (Exception ex)
            {
                //TODO: FogBug Logging
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public static CreateVisitorInformationResponse CreateVisitorInformation(CreateVisitorInformationRequest request)
        {
            CreateVisitorInformationResponse response = new CreateVisitorInformationResponse();
            string strTypeOfNumberSelected = "";
            int intNMCompanyID = 0;
            string strserviceNumber;
            try
            {
                if (ValidateVisitorInformation(request, response, out strserviceNumber))
                {
                    intNMCompanyID = CMSUtility.GetNMCompanyIDByWebID(request.webID);

                    if (intNMCompanyID > 0)
                    {
                        if (!string.IsNullOrEmpty(request.packageID))
                        {
                            Package objPackage = CMSUtility.GetPackageDetailsByPackageID(Convert.ToInt32(request.packageID), intNMCompanyID);
                            if (objPackage != null && !string.IsNullOrEmpty(objPackage.PackageID))
                            {
                                iTCPlan ObjiTCPlan = new iTCPlan();
                                //strpackageID = objPackage.Rows[0]["PackageID"].ToString();
                                //strPackageName = objPackage.Rows[0]["PackageName"].ToString();
                                //strserviceFee = Convert.ToString(objPackage.Rows[0]["ServiceFee"]);
                                //strsetUpFee = Convert.ToString(objPackage.Rows[0]["SetUpFee"]);
                                //strTCPlanID = objPackage.Rows[0]["PackageBaseServiceTCPlanID"].ToString();
                                // Creating the iTCPlan details
                                ObjiTCPlan.numberType = strTypeOfNumberSelected;
                                ObjiTCPlan.packageID = objPackage.PackageID;
                                ObjiTCPlan.packageName = objPackage.PackageName;
                                ObjiTCPlan.serviceFee = objPackage.ServiceFee;
                                ObjiTCPlan.setupFee = objPackage.SetUpFee;
                                ObjiTCPlan.serviceNumber = strserviceNumber;
                                ObjiTCPlan.baseserviceTCPlan = objPackage.PackageBaseServiceTCPlanID;
                                int intCMSOnlineHoldingID = 0;
                                string strEncodedCMSOnlineID = "";
                                // Updating the Initial signup information in CMSOnlineHoldingTbl
                                if (!string.IsNullOrEmpty(request.cmsOnlineID))
                                {
                                    strEncodedCMSOnlineID = request.cmsOnlineID;
                                    byte[] decodedBytes = Convert.FromBase64String(strEncodedCMSOnlineID);
                                    string strDecodedCMSOnlineID = System.Text.ASCIIEncoding.ASCII.GetString(decodedBytes);
                                    CMSUtility.UpdateVisitorInformation(Convert.ToInt32(strDecodedCMSOnlineID), request.voiceTranscription, ObjiTCPlan);
                                }
                                else
                                {
                                    // Inserting the Initial signup information in CMSOnlineHoldingTbl
                                    CMSUtility.InsertVistiorInfomation(request.webID, intNMCompanyID, request.ipAddress, request.voiceTranscription, ObjiTCPlan, ref intCMSOnlineHoldingID);
                                    byte[] encodedBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(intCMSOnlineHoldingID.ToString());
                                    strEncodedCMSOnlineID = Convert.ToBase64String(encodedBytes);

                                }
                                response.CMSOnlineID = strEncodedCMSOnlineID;
                                response.Status = Constants.ServiceResponses.Success;
                                response.ResponseMessage = Constants.ServiceResponses.SuccessResponseMessage;
                            }
                            else
                            {
                                response.Status = Constants.ServiceResponses.Error;
                                response.ResponseMessage = "Parameter mismatch.  The value provided for one or more parameters does not match the Resellers configuration";
                            }
                        }


                    }
                    else
                    {
                        response.Status = Constants.ServiceResponses.Error;
                        response.ResponseMessage = "Parameter mismatch.  The value provided for one or more parameters does not match the Resellers configuration";
                    }




                }

            }
            catch (Exception ex)
            {

                //TODO: FogBug Logging
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        private static bool ValidateVisitorInformation(CreateVisitorInformationRequest request, CreateVisitorInformationResponse response, out string strserviceNumber)
        {
            bool status = true;
            strserviceNumber = "";
            if (string.IsNullOrEmpty(request.webID))
            {
                status = false;
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = "Required parameters were not provided in the request";
            }
            if (!string.IsNullOrEmpty(request.cmsOnlineID))
            {
                byte[] decodedBytes = Convert.FromBase64String(request.cmsOnlineID);
                var strDecodedCMSOnlineID = System.Text.ASCIIEncoding.ASCII.GetString(decodedBytes);

                CMSOnlineHoldingTbl dtCMSDetails = CMSUtility.GetCMSOnlineDetailsByCMSOnlineID(Convert.ToInt32(strDecodedCMSOnlineID));

                if (dtCMSDetails != null && dtCMSDetails.TypeOfNumberSelected != "")
                {
                    strserviceNumber = dtCMSDetails.SelectedNumber;
                    if (request.webID.ToUpper() != dtCMSDetails.WebID)
                    {
                        status = false;
                        response.Status = Constants.ServiceResponses.Error;
                        response.ResponseMessage = "Parameter mismatch.  The value provided for one or more parameters does not match the Resellers configuration";
                    }
                }
                else
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Parameter mismatch.  The value provided for one or more parameters does not match the Resellers configuration";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.ipAddress))
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Required parameters were not provided in the request";
                }
                else
                {
                    if (!IsIPAddressValid(request.ipAddress))
                    {
                        status = false;
                        response.Status = Constants.ServiceResponses.Error;
                        response.ResponseMessage = "Parameter mismatch.  The value provided for one or more parameters does not match the Resellers configuration";
                    }
                }
            }

            if (string.IsNullOrEmpty(request.serviceNumber) && string.IsNullOrEmpty(request.numberType))
            {
                if (string.IsNullOrEmpty(request.packageID))
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Required parameters were not provided in the request";
                }
            }

            if (string.IsNullOrEmpty(request.packageID))
            {
                if (string.IsNullOrEmpty(request.serviceNumber) || string.IsNullOrEmpty(request.numberType))
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Required parameters were not provided in the request";
                }
            }

            if (!string.IsNullOrEmpty(request.serviceNumber))
            {
                if (string.IsNullOrEmpty(request.numberType))
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Required parameters were not provided in the request";
                }
                //else
                //{
                //    switch (request.numberType.ToUpper())
                //    {
                //        case "800":
                //            strTypeOfNumberSelected = "1";
                //            break;
                //        case "8XX":
                //            strTypeOfNumberSelected = "2";
                //            break;
                //        case "VANITY":
                //            strTypeOfNumberSelected = "3";
                //            break;
                //        case "LOCAL":
                //            strTypeOfNumberSelected = "4";
                //            break;
                //        case "TRANSFER":
                //            strTypeOfNumberSelected = "5";
                //            break;
                //        default:
                //            break;
                //    }

                //}


                if ((request.serviceNumber.All(char.IsDigit) == false) && (request.serviceNumber.Length != 10))
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Parameters format is invalid";
                }
                //else
                //{
                //    strserviceNumber = request.serviceNumber;
                //}
            }



            return status;
        }

        private static bool IsIPAddressValid(string ipAddress)
        {
            string strPattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            Match emailAddressMatch = Regex.Match(ipAddress, strPattern);
            if (emailAddressMatch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static GetStateResponse GetStatesByCountry(GetStateRequest request)
        {
            GetStateResponse response = new GetStateResponse();
            try
            {
                response.States = CMSUtility.GetStatesByCountry(request.countryAbbr);
                response.Status = Constants.ServiceResponses.Success;
                response.ResponseMessage = Constants.ServiceResponses.SuccessResponseMessage;
            }
            catch (Exception ex)
            {

                //TODO: FogBug Logging
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public static GetCountryResponse GetCountries(ServiceRequest request)
        {
            GetCountryResponse response = new GetCountryResponse();
            try
            {
                response.Countries = CMSUtility.GetCountryList();
                response.Status = Constants.ServiceResponses.Success;
                response.ResponseMessage = Constants.ServiceResponses.SuccessResponseMessage;
            }
            catch (Exception ex)
            {

                //TODO: FogBug Logging
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = ex.Message;
            }

            return response;

        }

        public static GetPackagesResponse GetPackages(ServiceRequest request)
        {
            GetPackagesResponse response = new GetPackagesResponse();

            try
            {
                if (string.IsNullOrEmpty(request.webID))
                {
                    request.webID = DefaultWebID;
                }
                int nmcompanyID = CMSUtility.GetNMCompanyIDByWebID(request.webID);
                if (nmcompanyID <= 0)
                {
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Parameter mismatch.  The value provided for one or more parameters does not match the Resellers configuration";
                }
                else
                {
                    var packages = CMSUtility.GetPackageDetailsByNMCompanyID(WebSiteID, nmcompanyID);
                    string[] pckTypes = new string[] { "BASIC", "PRO", "PREMIER" };
                    response.Packages = packages.Where(p => pckTypes.Contains(p.PackageName?.ToUpper())).ToList();
                    response.Status = Constants.ServiceResponses.Success;
                    response.ResponseMessage = Constants.ServiceResponses.SuccessResponseMessage;
                }
            }
            catch (Exception ex)
            {
                //TODO: FogBug Logging
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        private static bool ValidateRandomNumbersRequest(getRandomNumbersRequest request, getRandomNumbersResponse response)
        {
            bool status = true;
            Regex RegAreacode = new Regex(@"^[0-9]{3}$");
            Regex RegStAbbr = new Regex("^[a-zA-Z ]+$");
            if (string.IsNullOrEmpty(request.numberType))
            {
                status = false;
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = "Required parameters were not provided in the request";
            }
            else if (request.numberType.ToString() != "800" && request.numberType.ToUpper().ToString() != "8XX" && request.numberType.ToUpper().ToString() != "LOCAL")
            {
                status = false;
                response.Status = Constants.ServiceResponses.Error;
                response.ResponseMessage = "Parameters format is invalid";
            }
            else if (request.numberType.ToUpper() == "LOCAL")
            {
                if (string.IsNullOrEmpty(request.npa))
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Required parameters were not provided in the request";
                }
                else if (!RegAreacode.IsMatch(request.npa))
                {
                    status = false;
                    response.Status = Constants.ServiceResponses.Error;
                    response.ResponseMessage = "Parameters format is invalid";
                }
                else if (!string.IsNullOrEmpty(request.stateAbbr))
                {
                    if (!RegStAbbr.IsMatch(request.stateAbbr) || request.stateAbbr.Length != 2)
                    {
                        status = false;
                        response.Status = Constants.ServiceResponses.Error;
                        response.ResponseMessage = "Parameters format is invalid";
                    }
                }
            }
            return status;
        }
        private static void GetRandomNumbersList(getRandomNumbersRequest request, getRandomNumbersResponse response)
        {
            response.Numbers = new List<NumberInfo>();
            switch (request.numberType?.ToUpper())
            {
                case "LOCAL":
                    GetLocalRandomNumbers(request, response);
                    break;
                case "800":
                    Get800Numbers(request, response);
                    break;
                case "8XX":
                    Get8xxNumbers(request, response);
                    break;
                default:
                    break;
            }
        }

        private static void Get800Numbers(getRandomNumbersRequest request, getRandomNumbersResponse response)
        {
            var nums = CMSUtility.Get800PhoneNumbers(request.numberType);
            response.Numbers.AddRange(nums);
        }

        private static void Get8xxNumbers(getRandomNumbersRequest request, getRandomNumbersResponse response)
        {
            if (!string.IsNullOrEmpty(request.vanityKey))
            {
                string vanityKey = request.vanityKey;
                string strWildCard = "";
                if (vanityKey.Length > 6)
                {
                    vanityKey = vanityKey.Substring(0, 6);
                }
                string tmpvanityKey = vanityKey;
                int intRemLength = 7 - tmpvanityKey.Length;
                for (int i = 1; i <= intRemLength; i++)
                {
                    tmpvanityKey = "*" + tmpvanityKey;
                }
                strWildCard = request.npa + tmpvanityKey;

                while (response.Numbers.Count < request.qty)
                {
                    List<NumberInfo> nums = SomosUtility.GetVanityNumbers(strWildCard, vanityKey, request.qty);
                    response.Numbers.AddRange(nums);
                    vanityKey = vanityKey.Substring(0, vanityKey.Length - 1);
                    tmpvanityKey = "*" + tmpvanityKey.Substring(0, vanityKey.Length - 1);
                    strWildCard = request.npa + tmpvanityKey;

                    if (tmpvanityKey.Replace("*", "").Length < 4)
                    {
                        break;
                    }
                }
            }
            else
            {
                if (NumberFetchType == "1") //1:From Local DataBase Only
                {

                }
                else if (NumberFetchType == "2") //2: From IntelePeer Only
                {

                }
                else if (NumberFetchType == "3") //'3: From Local + IntelePeer
                {

                }
                else if (NumberFetchType == "4") //'4: From SOMOS API only  
                {
                    var data = SomosUtility.GetRandomNumbers(request.npa, request.nxx, request.qty);
                    if (response.Numbers.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            var find = response.Numbers.Find(f => f.phoneNumber?.ToUpper() == item.phoneNumber?.ToUpper());
                            if (find == null)
                            {
                                response.Numbers.Add(item);
                            }
                        }
                    }
                    else
                    {
                        response.Numbers.AddRange(data);
                    }
                }
            }
        }

        private static void GetLocalRandomNumbers(getRandomNumbersRequest request, getRandomNumbersResponse response)
        {
            var Numbers = new List<NumberInfo>();
            if (request.city != null)
            {
                if (!string.IsNullOrEmpty(request.stateAbbr))
                {
                    Numbers = IntelePeerUtility.TryToFetchAndAppendIntelePeerDIDNumbers(request.stateAbbr, request.city, "0");
                }
                else
                {
                    Numbers = IntelePeerUtility.TryToFetchAndAppendIntelePeerDIDNumbers("", request.city, "0");
                }
            }

            if (request.npa != null)
            {
                if (!string.IsNullOrEmpty(request.stateAbbr))
                {
                    Numbers = IntelePeerUtility.TryToFetchAndAppendIntelePeerDIDNumbers(request.stateAbbr, "", request.npa);
                }
                else
                {
                    Numbers = IntelePeerUtility.TryToFetchAndAppendIntelePeerDIDNumbers("", "", request.npa);
                }
            }
            response.Numbers.AddRange(Numbers);
        }
    }
}
