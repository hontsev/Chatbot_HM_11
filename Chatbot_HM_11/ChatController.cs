using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace Chatbot_HM_11
{
    public enum Meaning
    {
        ask,answer,statement,judge,special
    }
    public class ChatInfo
    {
        
        public int conceptIndex;

        public int loopWaitTime;
        public int loopOutputTime;
        public double like;
        public double excitement;
        public double interest;
        public int waitTime;
        public int maxWaitTime;

        public ChatInfo()
        {
            loopWaitTime = 100;
            loopOutputTime = 400;
            conceptIndex = 0;

            like = 50;
            excitement = 50;
            interest = 50;

            waitTime = 300;
            maxWaitTime = 5000;
        }
    }
    public class ChatController
    {
        public List<string> tmpInputSentence;
        public List<string> tmpOutputSentence;
        public List<ConceptUnit> concepts;
        public List<ConceptUnit> tmpConcepts;
        public ConceptUnit askConcept;

        public ChatInfo info;

        public int nowWaitTime;
        public int nowWaitAllTime;

        public List<List<string>> baiduWordList;


        public delegate void sendChatMessageDelegate(string res);



        public bool run;
        public sendChatMessageDelegate outputEvent;
        public sendChatMessageDelegate inputEvent;



        public List<List<string>> specials;

        public ChatController(sendChatMessageDelegate toutputEvent=null)
        {

            info = new ChatInfo();
            run = false;

            nowWaitTime = 0;
            nowWaitAllTime = 0;
            tmpInputSentence = new List<string>();
            tmpOutputSentence = new List<string>();

            concepts = new List<ConceptUnit>();
            askConcept = null;
            tmpConcepts = new List<ConceptUnit>();
            if(toutputEvent!=null)outputEvent = toutputEvent;
            inputEvent = new sendChatMessageDelegate(input);
            
            baiduWordList=IOController.readBaiduWords("BaiduWord.txt");
                
        }

        private int getNextConceptIndex()
        {
            info.conceptIndex++;
            return info.conceptIndex;
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

        private List<LtpWord> getWordByParent(List<LtpWord> words, int pid, string relate="")
        {
            List<LtpWord> ws = new List<LtpWord>();
            foreach (var w in words)
            {
                if (w.parent == pid && (relate.Length <= 0 || w.relate.Contains(relate)))
                {
                    ws.Add(w);
                }
            }
            return ws;
        }

        private List<LtpWord> getSubTreeWordsByParent(List<LtpWord> words, int pid)
        {
            List<LtpWord> ws = new List<LtpWord>();
            foreach (var w in words)
            {
                if (w.parent == pid && (w.relate == "ATT" || w.relate == "ADV" || w.relate == "CMP"))
                {
                    ws.Add(w);
                    foreach (var ww in getSubTreeWordsByParent(words,w.id))
                    {
                        ws.Add(ww);
                    }
                }
            }
            ws.Sort();
            return ws;
        }

        private List<ConceptUnit> getConcept(List<LtpWord> wordlist, LtpWord beginWord)
        {
            List<ConceptUnit> res = new List<ConceptUnit>();

            List<LtpWord> preds = new List<LtpWord>();
            preds.Add(beginWord);
            List<LtpWord> preds2 = getWordByParent(wordlist, beginWord.id, "COO");
            foreach (var w in preds2) preds.Add(w);

            List<LtpWord> lastsbvw = new List<LtpWord>();
            foreach (LtpWord predword in preds)
            {
                ConceptUnit u = new ConceptUnit();
                u.id = this.getNextConceptIndex();

                //设置谓语
                u.pred = new Word(predword);
                List<Word> apred = new List<Word>();
                //将谓语的修饰成分加入
                List<LtpWord> apredw = getSubTreeWordsByParent(wordlist, predword.id);
                foreach (var w in apredw)
                {
                    apred.Add(new Word(w));
                }
                //查询是否有主语
                List<LtpWord> sbvw = getWordByParent(wordlist, predword.id, "SBV");
                if (sbvw.Count > 0)
                {
                    lastsbvw = sbvw;
                }
                if (lastsbvw.Count > 0)
                {
                    //设置主语为上一个并列句的主语
                    u.sub = new Word(lastsbvw.First());
                    //设置主语修饰语
                    List<LtpWord> asbvw = getSubTreeWordsByParent(wordlist, lastsbvw.First().id);
                    List<Word> asub = new List<Word>();
                    foreach (var w in asbvw)
                    {
                        asub.Add(new Word(w));
                    }
                    u.asub = asub;
                }

                //查询是否有宾语
                List<LtpWord> objw = getWordByParent(wordlist, predword.id, "OB");
                if (objw.Count > 0)
                {
                    //首先将IOB即间接宾语设为谓语的修饰语
                    foreach (var w in objw)
                    {
                        if (w.pos == "IOB")
                        {
                            apred.Add(new Word(w));
                        }
                    }
                    u.apred = apred;
                    //然后针对其他宾语递归进行操作，并且根据宾语中心词词性选择是将其作为子句（记录id）还是作为实宾语
                    foreach (var w in objw)
                    {
                        if (w.pos != "IOB")
                        {
                            if (w.pos.Contains("n") 
                                || w.pos.Contains("a")
                                || (getWordByParent(wordlist, w.id, "SBJ").Count <= 0
                                  && getWordByParent(wordlist, w.id, "OB").Count <= 0))
                            {
                                //名词或形容词，作为实宾语
                                u.obj = new Word(w);
                                //设置宾语修饰语
                                List<Word> aobj = new List<Word>();
                                List<LtpWord> aobw = getSubTreeWordsByParent(wordlist, w.id);
                                foreach (var tw in aobw)
                                {
                                    aobj.Add(new Word(tw));
                                }
                                u.aobj = aobj;
                                break;
                            }
                            else
                            {
                                //将其作为子句进行递归
                                List<ConceptUnit> objConcepts = getConcept(wordlist, w);
                                foreach (ConceptUnit obju in objConcepts)
                                {
                                    //res.Add(obju);
                                    //concepts.Push(obju);
                                    if (u.obj.type.Length <= 0)
                                    {
                                        u.obj = new Word(obju.id.ToString(), "id");

                                    }
                                    else
                                    {
                                        //复制一份u以继承分析过的主谓
                                        ConceptUnit u2 = new ConceptUnit(u);
                                        u2.id = this.getNextConceptIndex();

                                        //concepts.Push(u2);
                                        u2.obj = new Word(obju.id.ToString(), "id");
                                        res.Add(u2);
                                        tmpConcepts.Add(u2);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    u.apred = apred;
                }
                //暂时只考虑复句的最内层。。
                if (u.obj.type != "id")
                {
                    tmpConcepts.Add(u);
                    res.Add(u);
                }

            }
            return res;
        }

        /// <summary>
        /// 将依存句法树转化为概念单元
        /// </summary>
        /// <param name="words"></param>
        private void getConcepts(List<LtpWord> words)
        {
            LtpWord beginw = getWordByParent(words, -1).First();
            tmpConcepts.Clear();
            getConcept(words, beginw);

            for (int i = 0; i < tmpConcepts.Count; i++)
            {
                if (tmpConcepts[i].sub.word.Contains( "你"))
                {
                    tmpConcepts[i].sub.word = tmpConcepts[i].sub.word.Replace("你", "我");
                }
                else if (tmpConcepts[i].sub.word.Contains("我"))
                {
                    tmpConcepts[i].sub.word = tmpConcepts[i].sub.word.Replace("我", "你");
                }
                if (tmpConcepts[i].obj.word.Contains("你"))
                {
                    tmpConcepts[i].obj.word = tmpConcepts[i].obj.word.Replace("你", "我");
                }
                else if (tmpConcepts[i].obj.word.Contains("我"))
                {
                    tmpConcepts[i].obj.word = tmpConcepts[i].obj.word.Replace("我", "你");
                }
            }

            //foreach (var c in tmpConcepts)
            //{
            //    string tmpstr = c.id + ":";
            //    foreach (var a in c.asub) tmpstr += a.word + " ";
            //    tmpstr += c.sub.word + " ";
            //    foreach (var a in c.apred) tmpstr += a.word + " ";
            //    tmpstr += c.pred.word + " ";
            //    foreach (var a in c.aobj) tmpstr += a.word + " ";
            //    tmpstr += c.obj.word;
            //    tmpOutputSentence.Add(tmpstr);
            //}

        }

        private string replaceImageWords(string str)
        {
            foreach (var match in baiduWordList)
            {
                str = str.Replace("<imgclass=\"word-replace\"src=\"http://zhidao.baidu.com/api/getdecpic?picenc=" + match[0] + "\">", match[1]);
            }
            return str;
        }

        private bool getWebsiteAnswer(string question)
        {
            bool isa = false;
            tmpOutputSentence.Add("我想想……");
            string askUrl = "http://www.baidu.com/s?wd=" + WebConnection.UrlEncode(question);
            string res = WebConnection.getData(askUrl,Encoding.UTF8);
            res = res.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            Regex reg = new Regex("class=\"op_exactqa_s_answer\">(.*?)</div>");
            if (reg.IsMatch(res))
            {
                //说明百度首页给出了智能答案
                res = reg.Match(res).Groups[1].ToString();
                reg = new Regex("target=\"_blank\">(.*?)</a>");
                if (reg.IsMatch(res))
                {
                    res = reg.Match(res).Groups[1].ToString();
                    tmpOutputSentence.Add(res);
                    isa = true;
                }
                
            }
            else
            {
                //判断是否是百度统计相关答案
                    reg = new Regex("<p class='op_gdp_subtitle'>(.*?)</p>");
                    if (reg.IsMatch(res))
                    {
                        res = reg.Match(res).Groups[1].ToString();
                        tmpOutputSentence.Add(res);
                        isa = true;
                    }
                    else
                    {
                        //判断是否是计算题答案
                        reg = new Regex("line-height:22px;padding-bottom:2px;width:474px;\">(.*?)</div>");
                        if (reg.IsMatch(res))
                        {
                            res = reg.Match(res).Groups[1].ToString().Replace("&nbsp;", " ");
                            tmpOutputSentence.Add(res);
                            isa = true;
                        }
                        else
                        {
                            //去百度知道查一波
                            askUrl = "http://zhidao.baidu.com/search?word=" + question;
                            res = WebConnection.getData(askUrl);
                            res = res.Replace("\n", "").Replace("\r", "").Replace(" ", "");

                            //如果rank较低就舍弃
                            reg = new Regex("data-rank=\"(.*?)\"");
                            if (reg.IsMatch(res))
                            {
                                string rank = reg.Match(res).Groups[1].ToString().Split(':')[0];
                                int rankvalue = Int32.Parse(rank);
                                if (rankvalue <= 750)
                                {
                                    //rank太低了，不再查询答案。
                                    return false;
                                }
                            }

                            reg = new Regex("data-log-area=\"list\">(.*?)</a>");
                            if (reg.IsMatch(res))
                            {
                                res = reg.Match(res).Groups[1].ToString();
                                reg = new Regex("href=\"(.*?)\"");
                                if (reg.IsMatch(res))
                                {
                                    //从知道首页找到最接近的答案的url
                                    askUrl = reg.Match(res).Groups[1].ToString();
                                    res = WebConnection.getData(askUrl).Replace("\n", "").Replace("\r", "").Replace(" ", "");
                                    reg = new Regex("class=\"wgt-best(.*?)class=\"answer-share-widget");
                                    if (reg.IsMatch(res))
                                    {
                                        res = reg.Match(res).Groups[1].ToString();
                                        reg = new Regex("<pre(.*?)>(.*?)</pre>");
                                        if (reg.IsMatch(res))
                                        {
                                            res = reg.Match(res).Groups[2].ToString();
                                            res = res.Replace("<br>", "\r\n");
                                            res = res.Replace("<br/>", "\r\n");
                                            res = replaceImageWords(res);
                                            tmpOutputSentence.Add(res);
                                            isa = true;
                                        }
                                    }
                                    else
                                    {
                                        //尝试优质答案
                                        reg = new Regex("class=\"quality-content-detailcontent\">(.*?)</div>");
                                        if (reg.IsMatch(res))
                                        {
                                            res = reg.Match(res).Groups[1].ToString();
                                            res = res.Replace("<br>", "\r\n");
                                            res = res.Replace("<br/>", "\r\n");
                                            res = replaceImageWords(res);
                                            tmpOutputSentence.Add(res);
                                            isa = true;
                                        }
                                        else
                                        {
                                            //尝试网友推荐答案
                                            reg = new Regex("class=\"wgt-recommend(.*?)class=\"answer-share-widget");
                                            if (reg.IsMatch(res))
                                            {
                                                res = reg.Match(res).Groups[1].ToString();
                                                reg = new Regex("<pre(.*?)>(.*?)</pre>");
                                                if (reg.IsMatch(res))
                                                {
                                                    res = reg.Match(res).Groups[2].ToString();
                                                    res = res.Replace("<br>", "\r\n");
                                                    res = res.Replace("<br/>", "\r\n");
                                                    res = replaceImageWords(res);
                                                    tmpOutputSentence.Add(res);
                                                    isa = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
            }
            
            
            

            return isa;
        }

        private bool isSpecialDeal(string str)
        {
            bool iss = false;

            foreach (var sentence in specials)
            {
                int currect = 0;
                for (int i = 0; i < sentence.Count - 1; i++)
                {
                    if (str.Contains(sentence[i])) currect++;
                }
                if (currect > 0 && (currect == sentence.Count - 1))
                {
                    tmpOutputSentence.Add(sentence.Last());
                    iss = true;
                    break;
                }
            }

            return iss;
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

            if (askConcept==null) return isa;


            string[] yeswords = { "改", "好", "行", "嗯", "ok", "OK", "哦" };
            string[] nowords = { "不", "别", "no", "NO" ,"甭", "算了" };

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
                    concepts.Add(new ConceptUnit(askConcept));
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

        private ConceptUnit searchConcept(ConceptUnit con,bool matchObj=false)
        {
            for (int i = concepts.Count - 1; i >= 0; i--)
            {
                ConceptUnit c = concepts[i];
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
                ConceptUnit cu = searchConcept(w,true);
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
                        askConcept = new ConceptUnit(w);
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
                    string[] whatWords={"什么","谁","哪"};
                    foreach(var word in whatWords)
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
                        ConceptUnit cu = searchConcept(w);
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
                        ConceptUnit cu = searchConcept(w, true);
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
            else if (isSpecialDeal(str))
            {
                //特殊词汇
            }
            else if (isQuestionDeal(str))
            {
                //疑问句
                if (LogicAskDeal(str))
                {
                    //从逻辑中找到答案
                }
                else if (getWebsiteAnswer(str))
                {
                    //答案从百度获取
                }
                else
                {
                    //百度没有这个答案
                    tmpOutputSentence.Add("这个……我不知道");
                    tmpOutputSentence.Add("你觉得呢？");
                    askConcept = new ConceptUnit(tmpConcepts.First());
                }
            }
            else
            {
                //分析句子逻辑
                LogicTellDeal(str);
            }

        }


        private void dealInput(string str)
        {
            //try
            //{
                string ltpres = WebConnection.getResultFromLtp(str);
                ltpres = ltpres.Replace("\n", "").Replace('\\', ' ').Replace(" ", "").Replace("[[[", "[").Replace("]]]", "]");
                List<LtpWord> words = IOController.DeserializeJsonToList<LtpWord>(ltpres);
                getConcepts(words);
                meaningAnalysis(str);
                IOController.saveInfoAsJson("chatbotInfo.json", this.info);
                IOController.saveConceptsAsJson("chatbotConcepts.json", this.concepts);
            //}
            //catch(Exception e)
            //{
            //    tmpOutputSentence.Add("(程序异常)" + e.Message);
            //}

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
