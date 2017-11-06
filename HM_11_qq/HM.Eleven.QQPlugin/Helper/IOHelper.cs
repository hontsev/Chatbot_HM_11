using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using HM.Eleven.QQPlugins.Struct;

namespace HM.Eleven.QQPlugins.Helper
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
            Encoding encoding = Encoding.UTF8;
            //Thread.Sleep(1000);
            using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                StreamReader reader = new StreamReader(file, encoding);
                string preContent = reader.ReadToEnd();
                reader.Dispose();
                return preContent;
            }
        }

        public static List<List<string>> readBaiduWords(string fileName)
        {
            Encoding encoding = Encoding.UTF8;
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
            Encoding encoding = Encoding.UTF8;
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




        
    }
}
