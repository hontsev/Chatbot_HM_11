using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HM.Eleven.QQPlugins.Actor
{
    public class FormatItem
    {
        public string value;
        public bool isParam;

        public FormatItem(string v,bool isp)
        {
            value = v;
            isParam = isp;
        }
    }
    public class LearnItem
    {
        public string question;
        public string answer;
        public DateTime time;
        public string author;

        public LearnItem()
        {

        }
        public LearnItem(string q,string a,string user,DateTime dt)
        {
            question = q;
            answer = a;
            time = dt;
            author = user;
        }
    }
    public class LearnActor
    {
        public List<LearnItem> items;
        string path = "learn.txt";

        public LearnActor()
        {
            this.items = new List<LearnItem>();
            init();
        }

        public void init()
        {
            items = new List<LearnItem>();
            try
            {
                if (!File.Exists(path)) File.Create(path);
                var tmp = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 0; i < tmp.Length - 3; i += 4)
                {
                    items.Add(new LearnItem(tmp[i], tmp[i + 1], tmp[i + 2], DateTime.Parse(tmp[i + 3])));
                }
            }
            catch
            {

            }
            

        }

        public void save()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < items.Count; i++)
                {
                    sb.Append(string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n", items[i].question, items[i].answer, items[i].author, items[i].time.ToShortDateString()));
                }
                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            catch
            {

            }

        }

        public void addLearn(string question,string answer,string author)
        {
            LearnItem item = new LearnItem(question, answer, author, DateTime.Now);
            items.Add(item);
        }

        public FormatItem[] getParams(string formatsen)
        {
            List<FormatItem> param = new List<FormatItem>();

            bool inner = false;
            string[] tmp1 = formatsen.Split(new char[] { '{' }, StringSplitOptions.None);
            
            foreach(var p in tmp1)
            {
                int index = p.IndexOf('}');
                if (index < 0)
                {
                    if (inner) param.Add(new FormatItem(p, true));
                    else param.Add(new FormatItem(p, false));
                    inner = !inner;
                }
                else
                {
                    param.Add(new FormatItem(p.Substring(0,index), true));
                    param.Add(new FormatItem(p.Substring(index+1), false));
                    inner = false;
                }
            }

            return param.ToArray();
        }

        public string fit(LearnItem item,string sentence)
        {

            string res = "";
            try
            {
                var param = getParams(item.question);
                string paramstr = "";
                for(int i = 0; i < param.Length; i++)
                {
                    if (!param[i].isParam) paramstr += param[i].value;
                    else
                    {
                        paramstr += "(?<a" + i + ">\\S+)";
                    }
                }

                Regex reg = new Regex(paramstr, RegexOptions.None);
                var regres = reg.Match(sentence);
                if (regres.Success)
                {
                    // match it
                    res = item.answer;
                    for (int i = 0; i < param.Length; i++)
                    {
                        if (param[i].isParam)
                        {
                            res = res.Replace("{" + param[i].value + "}", regres.Groups["a" + i].Value);
                        }
                    }
                    foreach (var p in param)
                    {

                    }

                }
                // TODO: 加入内置函数调用和声明
            }
            catch (Exception e)
            {

            }
           

            return res;
        }

        public string getAnswer(string sentence)
        {
            string res = "";
            foreach(var item in items)
            {
                var tmp = fit(item, sentence);
                if (!string.IsNullOrWhiteSpace(tmp))
                {
                    res = tmp;
                    break;
                }
            }
            return res;
        }

        public string deal(QQInfo info)
        {
            string sentence = info.info;
            int sym = sentence.IndexOf("》》");
            if (sym < 0)
            {
                // not found
                return getAnswer(sentence);
            }
            else
            {
                string res = "";

                string question = sentence.Substring(0, sym);
                string answer = sentence.Substring(sym + 2);
                string author = info.qq.ToString();
                addLearn(question, answer, author);

                return res;
            }
        }
        
    }
}
