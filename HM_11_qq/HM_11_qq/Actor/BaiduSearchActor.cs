using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HM_11_qq.Helper;
using HM_11_qq.Struct;
using System.Text.RegularExpressions;

namespace HM_11_qq.Actor
{
    class BaiduSearchActor
    {
        private List<List<string>> baiduWordList;

        public BaiduSearchActor()
        {
            init();
        }

        public void init()
        {
            baiduWordList = IOHelper.readBaiduWords("BaiduWord.txt");
        }

        private string replaceImageWords(string str)
        {
            foreach (var match in baiduWordList)
            {
                str = str.Replace("<imgclass=\"word-replace\"src=\"http://zhidao.baidu.com/api/getdecpic?picenc=" + match[0] + "\">", match[1]);
            }
            return str;
        }

        public string[] getWebsiteAnswer(string question)
        {
            List<string> answer = new List<string>();
            //tmpOutputSentence.Add("我想想……");
            string askUrl = "http://www.baidu.com/s?wd=" + WebConnectHelper.UrlEncode(question);
            string res = WebConnectHelper.getData(askUrl, Encoding.UTF8);
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
                    answer.Add(res);
                }

            }
            else
            {
                //判断是否是百度统计相关答案
                reg = new Regex("<p class='op_gdp_subtitle'>(.*?)</p>");
                if (reg.IsMatch(res))
                {
                    res = reg.Match(res).Groups[1].ToString();
                    answer.Add(res);
                }
                else
                {
                    //判断是否是计算题答案
                    reg = new Regex("line-height:22px;padding-bottom:2px;width:474px;\">(.*?)</div>");
                    if (reg.IsMatch(res))
                    {
                        res = reg.Match(res).Groups[1].ToString().Replace("&nbsp;", " ");
                        answer.Add(res);
                    }
                    else
                    {
                        string tmpstr="正在百度问题：" + question;
                        answer.Add(tmpstr);
                        //去百度知道查一波
                        askUrl = "http://zhidao.baidu.com/search?word=" + question;
                        res = WebConnectHelper.getData(askUrl);
                        res = res.Replace("\n", "").Replace("\r", "").Replace(" ", "");

                        //如果rank较低就舍弃
                        reg = new Regex("data-rank=\"(.*?)\"");
                        if (reg.IsMatch(res))
                        {
                            string rank = reg.Match(res).Groups[1].ToString().Split(':')[0];
                            int rankvalue = Int32.Parse(rank);
                            if (rankvalue <= 500)
                            {
                                //rank太低了，不再查询答案。
                                //return false;
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
                                res = WebConnectHelper.getData(askUrl).Replace("\n", "").Replace("\r", "").Replace(" ", "");

                                Regex[] regs = new Regex[]{
                                    //被采纳答案
                                    new Regex("wgt-best(.*?)i-quality-icon"),
                                    new Regex("wgt-best(.*?)answer-share-widget"),
                                    //尝试优质答案
                                    new Regex("quality-content-detailcontent\">(.*?)</div>"),
                                    //尝试网友推荐答案
                                    new Regex("wgt-recommend(.*?)i-quality-icon")

                                };
                                bool ismatch = false;
                                foreach (var treg in regs)
                                {
                                    if (treg.IsMatch(res))
                                    {
                                        //tmpOutputSentence.Add(res);
                                        res = treg.Match(res).Groups[1].ToString();
                                        ismatch = true;
                                        break;
                                    }
                                }
                                if (ismatch)
                                {
                                    reg = new Regex("<pre(.*?)>(.*?)</pre>");
                                    if (reg.IsMatch(res))
                                    {
                                        res = reg.Match(res).Groups[2].ToString();
                                        res = res.Replace("<br>", "\r\n");
                                        res = res.Replace("<br/>", "\r\n");
                                        res = replaceImageWords(res);
                                        answer.Add(res);
                                    }
                                }



                                //{

                                //    reg = ;
                                //    if (reg.IsMatch(res))
                                //    {
                                //        res = reg.Match(res).Groups[1].ToString();
                                //        res = res.Replace("<br>", "\r\n");
                                //        res = res.Replace("<br/>", "\r\n");
                                //        res = replaceImageWords(res);
                                //        tmpOutputSentence.Add(res);
                                //        isa = true;
                                //    }
                                //    else
                                //    {
                                //        ;
                                //        if (reg.IsMatch(res))
                                //        {
                                //            res = reg.Match(res).Groups[1].ToString();
                                //            reg = new Regex("<pre(.*?)>(.*?)</pre>");
                                //            if (reg.IsMatch(res))
                                //            {
                                //                res = reg.Match(res).Groups[2].ToString();
                                //                res = res.Replace("<br>", "\r\n");
                                //                res = res.Replace("<br/>", "\r\n");
                                //                res = replaceImageWords(res);
                                //                tmpOutputSentence.Add(res);
                                //                isa = true;
                                //            }
                                //        }
                                //    }
                                //}

                            }
                        }
                    }
                }
            }




            return answer.ToArray();
        }
    }
}
