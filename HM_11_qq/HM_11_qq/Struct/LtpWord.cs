using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM_11_qq.Struct
{
    public class LtpWord : IComparable
    {
        public int id;
        public string cont;
        public string pos;
        public int parent;
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
