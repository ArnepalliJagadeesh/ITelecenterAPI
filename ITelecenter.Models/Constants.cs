using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class Constants
    {
        public struct ServiceResponses
        {
            public const string Success = "SUCCESS";
            public const string Warning = "WARNING";
            public const string Error = "ERROR";
            public const string SuccessResponseMessage = "Successfully Retrived Data";
        }

        public struct IntelePeerInfo
        {
            

            public const string url_AuthToken = "/_rest/v3/authenticate";
            public const string url_ServiceAreaLookUp = "/_rest/v3/lookup/serviceArea/";   // ' Use this to check the Token validity also
                   
            public const string url_InventorySearch_800 = "/_rest/v3/carrier/tf";
            public const string url_InventorySearch_TF = "/_rest/v3/carrier/tf";
            public const string url_InventorySearch_Vanity = "";
            public const string url_InventorySearch_Local_DID = "/_rest/v3/carrier/did";
                   
            public const string url_Local_DID_Count = "/_rest/v3/carrier/did/count";
            public const string url_Local_DID_Fetch = "/_rest/v3/carrier/did";
                   
            public const string url_Reserve_TF_or_800_or_Vanity = "/_rest/v3/carrier/tf/reservations";
            public const string url_Reserve_Local_or_DID = "/_rest/v3/carrier/did/reservations";
                   
            public const string url_Purchase_TF_or_800_or_Vanity = "/_rest/v3/my/tf/orders";
            public const string url_Purchase_Local_or_DID = "/_rest/v3/my/did/orders";
                   
            public const string url_Disconnect_TF_or_800_or_Vanity = "/_rest/v3/my/tf/disconnect";
            public const string url_Disconnect_Local_or_DID = "/_rest/v3/my/did/disconnect";
                   
            public const string url_TF_Reserved_List = "/_rest/v3/carrier/tf/reservations";
            public const string url_DID_Reserved_List = "/_rest/v3/carrier/did/reservations";
        }
    }
}
