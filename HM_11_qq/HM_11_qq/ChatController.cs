using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using HM_11_qq.Helper;
using HM_11_qq.Struct;
using HM_11_qq.Actor;

namespace HM_11_qq
{    
    public class ChatController
    {
        public List<string> tmpInputSentence;
        public List<string> tmpOutputSentence;
        public List<Sentence> concepts;
        
        public Sentence askConcept;

        public ChatInfo info;

        public int nowWaitTime;
        public int nowWaitAllTime;


        //actors
        private BaiduSearchActor bsa;

        public delegate void sendChatMessageDelegate(string res);



        public bool run;
        public sendChatMessageDelegate outputEvent;
        public sendChatMessageDelegate inputEvent;



        public List<List<string>> specials;

        public ChatController(sendChatMessageDelegate toutputEvent = null)
        {

            info = new ChatInfo();
            run = false;

            nowWaitTime = 0;
            nowWaitAllTime = 0;
            tmpInputSentence = new List<string>();
            tmpOutputSentence = new List<string>();

            concepts = new List<Sentence>();
            askConcept = null;
            
            if (toutputEvent != null) outputEvent = toutputEvent;
            inputEvent = new sendChatMessageDelegate(input);

            bsa = new BaiduSearchActor();

            try
            {
                init();
            }
            catch
            {

            }

        }

        private void init()
        {
            if (concepts == null) concepts = new List<Sentence>();
            if (info == null) info = new ChatInfo();
        }



        public void start()
        {
            run = true;
            new Thread(thinkLoop).Start();
            new Thread(outputLoop).Start();
        }

        public void stop()
        {
            run = false;
        }

        public void input(string str)
        {
            tmpInputSentence.Add(str);
            nowWaitTime = 0;
        }


        private void removeConceptById(int id)
        {
            for (int i = 0; i < concepts.Count; i++)
            {
                if (concepts[i].id == id)
                {
                    concepts.RemoveAt(i);
                    break;
                }
            }
        }

        private bool isAnswerDeal(string str)
        {
            bool isa = false;

            if (askConcept == null) return isa;


            string[] yeswords = { "改", "好", "行", "嗯", "ok", "OK", "哦" };
            string[] nowords = { "不", "别", "no", "NO", "甭", "算了" };

            foreach (var w in nowords)
            {
                if (str.Contains(w))
                {
                    isa = true;
                    tmpOutputSentence.Add("哦，好吧。");
                    askConcept = null;
                    break;
                }
            }
            foreach (var w in yeswords)
            {
                if (str.Contains(w))
                {
                    isa = true;
                    tmpOutputSentence.Add("嗯，我记住了，" + askConcept.toString());
                    var oldc = searchConcept(askConcept, true);
                    if (oldc != null)
                    {
                        int id = searchConcept(askConcept, true).id;
                        removeConceptById(id);
                    }
                    concepts.Add(new Sentence(askConcept));
                    askConcept = null;
                    break;
                }
            }

            return isa;
        }

        private bool isQuestionDeal(string str)
        {
            bool isq = false;
            if (str.EndsWith("?")) isq = true;
            if (str.EndsWith("？")) isq = true;
            string[] questionWords = { "怎么样", "觉得", "认为", "知道", "吗", "哪", "呢", "么" };
            foreach (var w in questionWords)
            {
                if (str.Contains(w))
                {
                    isq = true;
                    break;
                }
            }

            return isq;
        }

        /// <summary>
        /// 计算两字符串的编辑距离
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static int LevenshteinDistance(string s1, string s2)
        {
            if (s1 == s2)
                return 0;
            else if (String.IsNullOrEmpty(s1))
                return s2.Length;
            else if (String.IsNullOrEmpty(s2))
                return s1.Length;

            var m = s1.Length + 1;
            var n = s2.Length + 1;
            var d = new int[m, n];

            // Step1
            for (var i = 0; i < m; i++) d[i, 0] = i;

            // Step2
            for (var j = 0; j < n; j++) d[0, j] = j;

            // Step3
            for (var i = 1; i < m; i++)
            {
                for (var j = 1; j < n; j++)
                {
                    var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;

                    var deletion = d[i - 1, j] + 1;
                    var insertion = d[i, j - 1] + 1;
                    var substitution = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(Math.Min(deletion, insertion), substitution);
                }
            }

            return d[m - 1, n - 1];
        }

        private Sentence searchConcept(Sentence con, bool matchObj = false)
        {
            for (int i = concepts.Count - 1; i >= 0; i--)
            {
                Sentence c = concepts[i];
                //主语
                string sbj1 = "";
                foreach (var p in c.asub) sbj1 += p.word;
                sbj1 += c.sub.word;
                string sbj2 = "";
                foreach (var p in con.asub) sbj2 += p.word;
                sbj2 += con.sub.word;
                //计算编辑距离
                int jl1 = LevenshteinDistance(sbj1, sbj2);
                if ((double)jl1 / Math.Min(sbj1.Length, sbj2.Length) > 0.2) continue;

                //谓语
                if (!c.pred.word.Contains(con.pred.word) && !con.pred.word.Contains(c.pred.word)) continue;

                if (matchObj)
                {
                    //宾语
                    string obj1 = "";
                    foreach (var p in c.aobj) obj1 += p.word;
                    obj1 += c.obj.word;
                    string obj2 = "";
                    foreach (var p in con.aobj) obj2 += p.word;
                    obj2 += con.obj.word;
                    //计算编辑距离
                    int jl2 = LevenshteinDistance(obj1, obj2);
                    if ((double)jl2 / Math.Min(obj1.Length, obj2.Length) > 0.2) continue;
                }


                return c;
            }
            return null;
        }




        private void LogicTellDeal(string str)
        {
            foreach (var w in tmpConcepts)
            {
                Sentence cu = searchConcept(w, true);
                if (cu != null)
                {
                    //tmpOutputSentence.Add("我记得：" + cu.toString() + "?");
                    int not1 = 0;
                    int not2 = 0;
                    foreach (var predw in cu.apred)
                    {
                        if (predw.word == "不") not1++;
                    }
                    foreach (var predw in w.apred)
                    {
                        if (predw.word == "不") not2++;
                    }
                    if (not1 == not2)
                    {
                        tmpOutputSentence.Add("嗯，是这样的。");
                    }
                    else
                    {
                        tmpOutputSentence.Add("不，我记得" + cu.toString());
                        tmpOutputSentence.Add("唔，这件事是我记错了吗……要改了它?");
                        askConcept = new Sentence(w);
                    }
                }
                else
                {
                    tmpOutputSentence.Add("好，我记住了，" + w.toString());
                    concepts.Add(w);
                }
            }
        }

        private bool LogicAskDeal(string str)
        {
            bool lga = false;

            foreach (var w in tmpConcepts)
            {


                //tmpOutputSentence.Add("我记得：" + cu.toString() + "?");
                bool isAskWhat = false;
                string[] whatWords = { "什么", "谁", "哪" };
                foreach (var word in whatWords)
                {
                    if (w.obj.word.Contains(word))
                    {
                        isAskWhat = true;
                        break;
                    }
                }
                if (isAskWhat)
                {
                    //是在询问一件事情
                    Sentence cu = searchConcept(w);
                    if (cu != null)
                    {
                        lga = true;
                        tmpOutputSentence.Add("我记得" + cu.toString());
                    }
                    else
                    {
                        lga = false;
                    }
                }
                else
                {
                    //是在证实一件事情
                    Sentence cu = searchConcept(w, true);
                    if (cu != null)
                    {
                        lga = true;
                        int not1 = 0;
                        int not2 = 0;
                        foreach (var predw in cu.apred)
                        {
                            if (predw.word == "不") not1++;
                        }
                        foreach (var predw in w.apred)
                        {
                            if (predw.word == "不") not2++;
                        }
                        if (not1 == not2)
                        {
                            tmpOutputSentence.Add("是的。");
                        }
                        else
                        {
                            tmpOutputSentence.Add("不，我记得" + cu.toString());
                        }
                    }
                    else
                    {
                        lga = false;
                    }
                }

            }

            return lga;
        }

        /// <summary>
        /// 意图分析
        /// </summary>
        private void meaningAnalysis(string str)
        {
            if (isAnswerDeal(str))
            {
                //回答上文的问题
            }
            else if (isQuestionDeal(str))
            {
                //疑问句
                if (LogicAskDeal(str))
                {
                    //从逻辑中找到答案
                }
                //else if (getWebsiteAnswer(str))
                //{
                //    //答案从百度获取
                //}
                else
                {
                    //百度没有这个答案
                    tmpOutputSentence.Add("这个……我不知道");
                    tmpOutputSentence.Add("你觉得呢？");
                    if(tmpConcepts.Count>0)
                        askConcept = new Sentence(tmpConcepts.First());
                }
            }
            else
            {
                //分析句子逻辑
                LogicTellDeal(str);
            }

        }




        private void thinkLoop()
        {
            while (run)
            {
                info.excitement -= 1;
                if (this.tmpInputSentence.Count <= 0)
                {
                    nowWaitAllTime = 0;
                    nowWaitTime = 0;
                }
                else
                {
                    this.nowWaitTime += info.loopWaitTime;
                    this.nowWaitAllTime += info.loopWaitTime;
                    if (nowWaitTime > info.waitTime || nowWaitAllTime > info.maxWaitTime)
                    {
                        nowWaitAllTime = 0;
                        nowWaitTime = 0;
                        for (int i = 0; i < tmpInputSentence.Count; i++)
                        {
                            dealInput(tmpInputSentence[0]);
                            tmpInputSentence.RemoveAt(0);
                            //ltp的api调用间隔为200ms
                            Thread.Sleep(210);
                        }
                    }
                }

                Thread.Sleep(info.loopWaitTime);
            }
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

                Thread.Sleep(info.loopOutputTime);
            }
        }
    }
}
