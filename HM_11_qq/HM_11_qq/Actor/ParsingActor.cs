using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HM_11_qq.Struct;
using HM_11_qq.Helper;

namespace HM_11_qq.Actor
{
    /// <summary>
    /// 语法分析模块
    /// </summary>
    class ParsingActor
    {
        private int conceptIndex = 0;
        public List<Sentence> tmpConcepts;

        public ParsingActor()
        {
            tmpConcepts = new List<Sentence>();
        }

        public Sentence[] parsing(string str)
        {
            string ltpres = WebConnectHelper.getResultFromLtp(str);

            
            List<LtpWord> words = IOHelper.DeserializeJsonToConceptList(ltpres);
            getConcepts(words);
            meaningAnalysis(str);
        }

        private int getNextConceptIndex()
        {
            conceptIndex++;
            return conceptIndex;
        }


        private List<LtpWord> getWordByParent(List<LtpWord> words, int pid, string relate = "")
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
                    foreach (var ww in getSubTreeWordsByParent(words, w.id))
                    {
                        ws.Add(ww);
                    }
                }
            }
            ws.Sort();
            return ws;
        }

        private List<Sentence> getConcept(List<LtpWord> wordlist, LtpWord beginWord)
        {
            List<Sentence> res = new List<Sentence>();

            List<LtpWord> preds = new List<LtpWord>();
            preds.Add(beginWord);
            List<LtpWord> preds2 = getWordByParent(wordlist, beginWord.id, "COO");
            foreach (var w in preds2) preds.Add(w);

            List<LtpWord> lastsbvw = new List<LtpWord>();
            foreach (LtpWord predword in preds)
            {
                Sentence u = new Sentence();
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
                                List<Sentence> objConcepts = getConcept(wordlist, w);
                                foreach (Sentence obju in objConcepts)
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
                                        Sentence u2 = new Sentence(u);
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
                if (tmpConcepts[i].sub.word.Contains("你"))
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
    }
}
