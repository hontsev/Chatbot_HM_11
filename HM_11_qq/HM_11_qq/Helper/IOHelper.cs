using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using HM_11_qq.Struct;

namespace HM_11_qq.Helper
{
    class IOHelper
    {
        /// <summary>
        /// 读取单个txt文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string readTxtFile(string fileName)
        {
            Encoding encoding = Encoding.Default;
            //Thread.Sleep(1000);
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                StreamReader reader = new StreamReader(file, encoding);
                string preContent = reader.ReadToEnd();
                reader.Dispose();
                return preContent;
            }
        }

        public static List<List<string>> readBaiduWords(string fileName)
        {
            Encoding encoding = Encoding.Default;
            List<List<string>> baiduWords = new List<List<string>>();
            using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                StreamReader reader = new StreamReader(file, encoding);
                while (true)
                {
                    string res = reader.ReadLine();
                    if (String.IsNullOrEmpty(res)) break;
                    List<string> s = new List<string>(res.Split(' '));
                    baiduWords.Add(s);
                }
                reader.Dispose();
            }
            return baiduWords;
        }

        public static List<List<string>> readSpecialAnswerFromFile(string fileName)
        {
            Encoding encoding = Encoding.Default;
            List<List<string>> specials = new List<List<string>>();
            using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                StreamReader reader = new StreamReader(file, encoding);
                while (true)
                {
                    string res = reader.ReadLine();
                    if (String.IsNullOrEmpty(res)) break;
                    List<string> s = new List<string>(res.Split(' '));
                    specials.Add(s);
                }
                reader.Dispose();
            }
            return specials;
        }






        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        //public static List<T> DeserializeJsonToList<T>(string json) where T : class
        //{
        //    JsonSerializer serializer = new JsonSerializer();
        //    StringReader sr = new StringReader(json);
        //    object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
        //    List<T> list = o as List<T>;
        //    return list;
        //}

        public static List<LtpWord> DeserializeJsonToConceptList(string json)
        {
            json = json.Replace("\n", "").Replace('\\', ' ').Replace(" ", "").Replace("[[[", "").Replace("]]]", "");

            List<LtpWord> list = new List<LtpWord>();

            foreach (Match wordstr in Regex.Matches(json,@"{.*?}"))
            {
                LtpWord w = new LtpWord();

                Regex reg = new Regex("\\\"id\\\":(.*?),");
                MatchCollection  m ;
                m = reg.Matches(wordstr.Value);
                if (0 < m.Count) w.id = Int32.Parse(m[0].Groups[1].ToString());

                reg = new Regex("\\\"parent\\\":(.*?),");
                m = reg.Matches(wordstr.Value);
                if (0 < m.Count) w.parent = Int32.Parse(m[0].Groups[1].ToString());

                reg = new Regex("\\\"cont\\\":\\\"(.*?)\\\"");
                m = reg.Matches(wordstr.Value);
                if (0 < m.Count) w.cont = m[0].Groups[1].ToString();

                reg = new Regex("\\\"pos\\\":\\\"(.*?)\\\"");
                m = reg.Matches(wordstr.Value);
                if (0 < m.Count) w.pos = m[0].Groups[1].ToString();

                reg = new Regex("\\\"relate\\\":\\\"(.*?)\\\"");
                m = reg.Matches(wordstr.Value);
                if (0 < m.Count) w.relate = m[0].Groups[1].ToString();

                list.Add(w);
            }
            
            return list;
        }

        //public static ChatInfo getInfoFromJson(string fileName)
        //{
        //    ChatInfo c = null;
        //    Encoding encoding = Encoding.Default;
        //    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate))
        //    {
        //        StreamReader reader = new StreamReader(file, encoding);
        //        string str = reader.ReadToEnd();
        //        JsonSerializer serializer = new JsonSerializer();
        //        StringReader sr = new StringReader(str);
        //        object tc = serializer.Deserialize(new JsonTextReader(sr));
        //        c = tc as ChatInfo;
        //        reader.Dispose();
        //    }
        //    return c;
        //}

        //public static List<ConceptUnit> getConceptsFromJson(string fileName)
        //{
        //    List<ConceptUnit> c = null;
        //    Encoding encoding = Encoding.Default;
        //    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate))
        //    {
        //        StreamReader reader = new StreamReader(file, encoding);
        //        string str = reader.ReadToEnd();
        //        JsonSerializer serializer = new JsonSerializer();
        //        StringReader sr = new StringReader(str);
        //        object tc = serializer.Deserialize(new JsonTextReader(sr), typeof(List<ConceptUnit>));
        //        c = tc as List<ConceptUnit>;
        //        reader.Dispose();
        //    }
        //    return c;
        //}

        //public static void saveInfoAsJson(string fileName, ChatInfo info)
        //{

        //    string saveJsonString = JsonConvert.SerializeObject(info);
        //    using (FileStream file = new FileStream(fileName, FileMode.Create))
        //    {
        //        StreamWriter writer = new StreamWriter(file, Encoding.UTF8);
        //        writer.Write(saveJsonString);
        //        writer.Close();
        //    }
        //}

        //public static void saveConceptsAsJson(string fileName, List<ConceptUnit> info)
        //{

        //    string saveJsonString = JsonConvert.SerializeObject(info);
        //    using (FileStream file = new FileStream(fileName, FileMode.Create))
        //    {
        //        StreamWriter writer = new StreamWriter(file, Encoding.UTF8);
        //        writer.Write(saveJsonString);
        //        writer.Close();
        //    }
        //}
    }
}
