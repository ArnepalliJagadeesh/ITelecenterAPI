using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ITelecenter.Utility
{
    public class QueryStringBuilder : Dictionary<string, List<string>>
    {
        public void AddItem(string key, string value)
        {
            List<string> values = new List<string>();

            if (this.TryGetValue(key, out values))
                values.Add(value);
            else
                this[key] = new List<string>() { value };
        }

        public override string ToString()
        {
            string concat = "&";
            List<string> values = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (string key in this.Keys)
            {
                values = this[key];

                switch (values.Count())
                {
                    case 0:
                        {
                            break;
                        }

                    case 1:
                        {
                            sb.AppendFormat("{0}={1}{2}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(values[0]), concat);
                            break;
                        }

                    default:
                        {
                            foreach (string value in values)
                                sb.AppendFormat("{0}[]={1}{2}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value), concat);
                            break;
                        }
                }
            }

            sb.Remove(sb.Length - concat.Length, concat.Length);
            return sb.ToString();
        }
    }
}
