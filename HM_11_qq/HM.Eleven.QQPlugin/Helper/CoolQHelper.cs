using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


        public static long getAt(string str)
        {
            Regex reg = new Regex(@"\[CQ:at,qq=([0-9]+?)\]");
            var res = reg.Match(str);
            if (res.Success)
            {
                long qq = long.Parse(res.Groups[1].ToString());
                return qq;
            }
            return 0;
        }

        public static string cleanCQAt(string str)
        {
            string res = str;
            Regex reg = new Regex(@"\[CQ:at.*?\]");
            res = reg.Replace(str, "");
            return res;
        }

        public static string cleanCQCode(string str)
        {
            string res = str;
            Regex reg = new Regex(@"\[CQ:.*?\]");
            res = reg.Replace(str, "");
            return res;
        }
    }
}
