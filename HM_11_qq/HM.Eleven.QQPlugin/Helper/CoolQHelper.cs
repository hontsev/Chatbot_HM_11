using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM.Eleven.QQPlugins.Helper
{
    class CoolQHelper
    {
        private static string format(string str)
        {
            return str.Replace("&", "&amp;").Replace("[", "&#91;").Replace("]", "&#93;").Replace(",", "&#44;");
        }

        public static string sendPicture(string filepath)
        {
            return "[CQ:image,file=" + format(filepath) + "]";
        }

        public static string sendAt(long QQ)
        {
            return "[CQ:at,qq=" + QQ + "]";
        }

        public static string sendSound(string filepath)
        {
            return "[CQ:record,file=" + format(filepath) + ",magic=false]";
        }

        public static string sendString(string str)
        {
            return str.Replace("&", "&amp;").Replace("[", "&#91;").Replace("]", "&#93;");
        }
    }
}
