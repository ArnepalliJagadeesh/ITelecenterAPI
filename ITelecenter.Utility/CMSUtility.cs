using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITelecenter.Models;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Data;

namespace ITelecenter.Utility
{
    public class CMSUtility
    {
        public static readonly string constring = ConfigurationManager.AppSettings["CMSCONNECTIONSTRING"].ToString();
        public static readonly string SOMOS_RespOrgID = ConfigurationManager.AppSettings["SOMOS_RespOrgID"];
        public static SomosOAuthToken GetSomosOAuthToken()
        {
            SomosOAuthToken token;
            using (SqlConnection con = new SqlConnection(constring))
            {
                token = con.Query<SomosOAuthToken>("sp_FetchCMSVarSomosOAuthToken", commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return token;
        }
        public static void UpdateSomosOAuthToken(string strOAuthToken)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                var p = new DynamicParameters();
                p.Add("nvaOAuthToken", strOAuthToken);
                con.Execute("sp_UpdateCMSVarSomosOAuthToken", p, commandType: CommandType.StoredProcedure);
            }
        }

        public static List<NumberInfo> Get800PhoneNumbers(string numberType)
        {
            List<NumberInfo> numbrs = new List<NumberInfo>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                var p = new DynamicParameters();
                p.Add("nvaVisitedNumbers", numberType);
                numbrs = con.Query<NumberInfo>("spNumberInventory_Get800Numbers", p, commandType: CommandType.StoredProcedure).ToList();
            }
            numbrs.ForEach(n => n.respOrg = SOMOS_RespOrgID);
            return numbrs;
        }

        public static List<StateModel> GetStatesByCountry(string countryAbbr)
        {
            List<StateModel> states = new List<StateModel>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                var p = new DynamicParameters();
                p.Add("intCountryID", countryAbbr);
                states = con.Query<StateModel>("spState_SelectStateByCountryID", p, commandType: CommandType.StoredProcedure).ToList();
            }
            return states;
        }

        public static Package GetPackageDetailsByPackageID(int packageID, int NMCompanyID)
        {
            Package package;
            using (SqlConnection con = new SqlConnection(constring))
            {
                var p = new DynamicParameters();
                p.Add("intPackageID", packageID);
                p.Add("intNMCompanyID", NMCompanyID);
                package = con.Query<Package>("spPackage_FetchPackageDetailsByPackageID", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return package;
        }

        public static List<CountryModel> GetCountryList()
        {
            List<CountryModel> countries = new List<CountryModel>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                countries = con.Query<CountryModel>("spCountry_SelectCountry", commandType: CommandType.StoredProcedure).ToList();
            }
            return countries;
        }

        public static int GetNMCompanyIDByWebID(string strWebID)
        {
            int intNMCompanyID;
            using (SqlConnection con = new SqlConnection(constring))
            {
                var p = new DynamicParameters();
                p.Add("WebID", strWebID);
                intNMCompanyID = Convert.ToInt32(con.ExecuteScalar("spNmCompany_FetchNMCompanyIDByWebID", p, commandType: CommandType.StoredProcedure));
            }
            return intNMCompanyID;

        }

        public static CMSOnlineHoldingTbl GetCMSOnlineDetailsByCMSOnlineID(int v)
        {
            throw new NotImplementedException();
        }

        public static List<Package> GetPackageDetailsByNMCompanyID(object websiteID, int nmcompanyID)
        {
            List<Package> packages = new List<Package>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                var p = new DynamicParameters();
                p.Add("intWebSiteID", nmcompanyID);
                p.Add("intNMCompanyID", nmcompanyID);
                packages = con.Query<Package>("spPackage_SelectPackageDetailsByNMCompanyIDAndWebSiteID", p, commandType: CommandType.StoredProcedure).ToList();
            }
            return packages;
        }

        public static void UpdateVisitorInformation(int v, int voiceTranscription, iTCPlan objiTCPlan)
        {
            throw new NotImplementedException();
        }

        public static void InsertVistiorInfomation(string webID, int intNMCompanyID, string ipAddress, int voiceTranscription, iTCPlan objiTCPlan, ref int intCMSOnlineHoldingID)
        {
            throw new NotImplementedException();
        }
    }
}
