using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Models
{
    public class SomosResponse
    {
    }

    public class ErrList
    {
        public string errMsg { get; set; }
        public string errCode { get; set; }
        public string errLvl { get; set; }
    }

    public class RandomNumbers
    {
        public List<string> numList { get; set; }
        public List<ErrList> errList { get; set; }
        public int reqId { get; set; }
        public int blkId { get; set; }
    }

    public class QueryResult
    {
        public string num { get; set; }
        public string lastActDt { get; set; }
        public string resUntilDt { get; set; }
        public string discUntilDt { get; set; }
        public string effDt { get; set; }
        public string status { get; set; }
        public string ctrlRespOrgId { get; set; }
        public string conName { get; set; }
        public string conPhone { get; set; }
        public string shrtNotes { get; set; }
        public string recVersionId { get; set; }
        public List<ErrList> errList { get; set; }
    }

    public class QueryNumber
    {
        public List<QueryResult> queryResult { get; set; }
        public int reqId { get; set; }
        public int blkId { get; set; }
        public string subDtTm { get; set; }
    }

    public class PointerRecordResponse
    {
        public string num { get; set; }
        public string effDtTm { get; set; }
        public List<ErrList> errList { get; set; }
        public string recVersionId { get; set; }
        public int reqId { get; set; }
    }
}
