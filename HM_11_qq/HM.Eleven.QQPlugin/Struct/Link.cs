using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM.Eleven.QQPlugins.Struct
{

    public class Link
    {
        public string id1;
        public string id2;
        public DateTime date;
        public string author;
        public string type;
        public Link(string id1, string id2, string type, string date, string author)
        {
            this.id1 = id1;
            this.id2 = id2;
            this.type = type;
            this.author = author.Replace("-", " ");
            string strdate = date;
            string format = "yyyyMMddHHmmss";
            DateTime.TryParseExact(strdate, format,
                               System.Globalization.CultureInfo.InvariantCulture,
                               System.Globalization.DateTimeStyles.None, out this.date);

        }

    }

    public class Area
    {
        public string id;

        public Area()
        {

        }
    }
}
