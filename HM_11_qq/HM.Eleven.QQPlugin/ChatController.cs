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
    public class QQInfo
    {
        public string info;
        public long qq;
        public string nickname;
        public bool isgroup;

        public QQInfo(string _info,long _qq,bool _isgroup = false)
        {
            info = _info;
            qq = _qq;
            isgroup = _isgroup;
        }
    }
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

        private bool isRelate(string sentence)
        {
            return sentence.IndexOf("苦瓜") == 0;
        }

        private bool isQuestion(string sentence)
        {
            if (sentence.Last()=='?' || sentence.Last()=='？' ) return true;
            return false;
        }


        

        public void bkgInput(object oinfo)
        {
            var info = (QQInfo)oinfo;
            string sentence = info.info;

            if (info.isgroup && !isRelate(sentence)) return;
            else
            {
                if(info.isgroup) sentence = sentence.Substring(2);
            }


            if (info.info == "-保存学习数据")
            {
                this.la.save();
                return;
            }
            info.info = sentence;
            string res = la.deal(info);
            if (!string.IsNullOrWhiteSpace(res))
            {
                tmpQQOutput.Add(new QQInfo(res, info.qq, info.isgroup));
                return;
            }


            if (isQuestion(sentence))
            {
                //是问句
                sentence = sentence.Substring(0, sentence.Length - 1);
                string[] answers = BaiduSearchActor.getBaiduKGResult(sentence);
                if(answers.Length>0)  foreach (var s in answers) tmpQQOutput.Add(new QQInfo(s, info.qq, info.isgroup));
                else
                {
                    //没查到
                    answers = BaiduSearchActor.getBaiduZhidaoAnswers(info.info);
                    foreach (var s in answers)
                    {
                        tmpQQOutput.Add(new QQInfo(s, info.qq, info.isgroup));// break;
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
