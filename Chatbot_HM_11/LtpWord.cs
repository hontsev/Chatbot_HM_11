using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chatbot_HM_11
{
    public class LtpWord : IComparable
    {
        [JsonProperty(PropertyName = "id")]
        public int id;
        [JsonProperty(PropertyName = "cont")]
        public string cont;
        [JsonProperty(PropertyName = "pos")]
        public string pos;
        [JsonProperty(PropertyName = "parent")]
        public int parent;
        [JsonProperty(PropertyName = "relate")]
        public string relate;

        public int CompareTo(object obj)
        {
            int result;
            try
            {
                LtpWord info = obj as LtpWord;
                if (this.id > info.id)
                {
                    result = 1;
                }
                else
                {
                    result = -1;
                }
                return result;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
