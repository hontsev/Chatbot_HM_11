using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using HM.Eleven.QQPlugins.Helper;
using HM.Eleven.QQPlugins.Struct;
using HM.Eleven.QQPlugins.Actor;

namespace HM.Eleven.QQPlugins
{
    public class ChatController
    {
        bool run = true;
        public List<string> tmpOutputSentence;
        public List<QQInfo> tmpQQOutput;

        public delegate void sendChatMessageDelegate(string str);
        public delegate void sendQQChatMessage(QQInfo info);
        public sendChatMessageDelegate outputEvent;
        public sendQQChatMessage outputQQEvent;
        LearnActor la;

        public ChatController()
        {
            tmpOutputSentence = new List<string>();
            tmpQQOutput = new List<QQInfo>();
            la = new LearnActor();
        }

        public void start()
        {
            run = true;
            new Thread(outputLoop).Start();
        }
        public void stop()
        {
            run = false;
        }

        public string getMessageImme(string inputstr)
        {
            if (!isRelate(new QQInfo(inputstr,0,0))) return "";
            dealSentence(inputstr);
            if (this.tmpOutputSentence.Count > 0)
            {
                string res= tmpOutputSentence[0];
                tmpOutputSentence.Clear();
                return res;
                
            }
            return "";
        }

        private void dealSentence(string str)
        {
            string[] tmp = ItemParser.splitSentence(str);
            StringBuilder sb = new StringBuilder();
            foreach (var v in tmp)
            {
                string[] res=BaiduSearchActor.getBaiduKGResult(v);
                if (res.Length <= 0)
                {
                    res = BaiduSearchActor.getBaiduZhidaoAnswers(v);
                }
                if (res.Length > 0)
                {
                    foreach (var s in res)
                    {
                        tmpOutputSentence.Add(s); break;
                    }
                }
                //BaiduSearchActor.getSearchResult(v);
            }
            //tmpOutputSentence.Add(sb.ToString());
        }
        public void bkgInputStr(object ostr)
        {
            var str = (string)ostr;
            dealSentence(str);
        }


        /// <summary>
        /// 该语句是否与自己相关？主要用于群聊
        /// 顺便将判断用的字段删掉
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool isRelate(QQInfo info)
        {
            bool hasat = false;
            if (info.isGroup)
            {
                // at
                if (CoolQHelper.getAt(info.info) == info.selfQQ) hasat = true;
                info.info = CoolQHelper.cleanCQAt(info.info);
            }
            else
            {
                hasat = true;
            }
            //Regex reg=new Regex(@"苦瓜")
            bool hasstr = info.info.IndexOf("苦瓜") == 0;
            if (hasstr)
            {
                info.info = info.info.Substring(2);
            }
            return hasat || hasstr;
        }


        private bool isQuestion(string sentence)
        {
            if (sentence.Last()=='?' || sentence.Last()=='？' ) return true;
            return false;
        }


        

        public void bkgInput(object oinfo)
        {
            var info = (QQInfo)oinfo;
            

            if (!isRelate(info)) return;

            if (info.info == "-保存")
            {
                this.la.save();
                tmpQQOutput.Add(new QQInfo("以保存", info.fromQQ, info.isGroup));
                return;
            }
            //info.info = sentence;
            string sentence = info.info;

            string res = la.deal(info);
            if (!string.IsNullOrWhiteSpace(res.Trim()))
            {
                tmpQQOutput.Add(new QQInfo(res, info.fromQQ, info.isGroup));
                return;
            }

            
            if (isQuestion(sentence))
            {
                //是问句
                //tmpQQOutput.Add(new QQInfo("百度："+sentence, info.fromQQ, info.isGroup));
                sentence = sentence.Substring(0, sentence.Length - 1);
                string[] answers = BaiduSearchActor.getBaiduKGResult(sentence);
                if(answers.Length>0)  foreach (var s in answers) tmpQQOutput.Add(new QQInfo(s, info.fromQQ, info.isGroup));
                else
                {
                    //没查到
                    answers = BaiduSearchActor.getBaiduZhidaoAnswers(sentence);
                    foreach (var s in answers)
                    {
                        //if (!string.IsNullOrWhiteSpace(s.Trim()))
                            tmpQQOutput.Add(new QQInfo(s, info.fromQQ, info.isGroup));// break;
                    }
                }

            }

        }

        public void input(QQInfo info)
        {
            //string[] tmp = ItemParser.splitSentence(info.);
            new Thread(bkgInput).Start((object)info);



            //BaiduSearchActor.getSearchResult(v);


        }

        public void input(string str)
        {
            new Thread(bkgInputStr).Start((object)str);
            //tmpOutputSentence.Add(str);
        }

        private void outputLoop()
        {
            while (run)
            {
                if (tmpOutputSentence.Count >= 1)
                {
                    string outputstr = tmpOutputSentence[0];
                    tmpOutputSentence.RemoveAt(0);
                    outputEvent(outputstr);
                }
                if (tmpQQOutput.Count >= 1)
                {
                    var outputinfo = tmpQQOutput[0];
                    outputQQEvent(outputinfo);
                    tmpQQOutput.RemoveAt(0);
                }
                Thread.Sleep(2000);
            }
        }
    }
}
