using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HM.Eleven.QQPlugins;
using System.Security.Cryptography;
using System.Text;

namespace WechatConn.Controllers
{
    public class ValuesController : ApiController
    {
        public ChatController cc;

        private string getMessage(string inputStr)
        {
            cc = new ChatController();
            //cc.outputEvent = new ChatController.sendChatMessageDelegate(printOutput);
            //cc.start();
            return cc.getMessageImme(inputStr);
        }


        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    string token = "loyung";
        //    if (string.IsNullOrWhiteSpace(token))
        //    {
        //        return null;
        //    }


        //    return new string[] { "value1", "value2" };
        //}

        //public string EncryptToSHA1(string str)
        //{
        //    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        //    byte[] str1 = Encoding.UTF8.GetBytes(str);
        //    byte[] str2 = sha1.ComputeHash(str1);
        //    sha1.Clear();
        //    (sha1 as IDisposable).Dispose();
        //    return Convert.ToBase64String(str2);
        //}



        //public HttpResponseMessage Get(string signature, string timestamp, string nonce,string echostr)
        //{
        //    string token = "yuki";
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return null;
        //    }

        //    if (!string.IsNullOrEmpty(echostr))
        //    {
        //        return new HttpResponseMessage { Content = new StringContent(echostr, Encoding.GetEncoding("UTF-8"), "text/plain") };
        //    }

        //    string[] tmparr = new string[] { token, nonce, timestamp };
        //    Array.Sort(tmparr, string.CompareOrdinal);
        //    string test = "";
        //    foreach (var s in tmparr) test += s;
        //    var res = EncryptToSHA1(test);
        //    string output = "";
        //    if (res != signature) output= echostr;
        //    else output= echostr;

        //    HttpResponseMessage responseMessage = new HttpResponseMessage { Content = new StringContent(output, Encoding.GetEncoding("UTF-8"), "text/plain") };

        //    return responseMessage;
        //}
        public HttpResponseMessage Get(string signature, string timestamp, string nonce, string echostr)
        {
            var result = new StringContent(echostr, UTF8Encoding.UTF8, "application/x-www-form-urlencoded");
            if (CheckSource(signature, timestamp, nonce))
            {
                result = new StringContent(echostr, UTF8Encoding.UTF8, "application/x-www-form-urlencoded");
                
            }
            var response = new HttpResponseMessage { Content = result };
            return response;
        }

        //检验是否来自微信的签名
        public bool CheckSource(string signature, string timestamp, string nonce)
        {
            var str = string.Empty;
            var token = "fluoreyuki";
            var parameter = new List<string> { token, timestamp, nonce };
            parameter.Sort();
            var parameterStr = parameter[0] + parameter[1] + parameter[2];
            var tempStr = GetSHA1(parameterStr).Replace("-", "").ToLower();
            if (tempStr == signature)
                return true;

            return false;
        }

        //SHA1加密
        public string GetSHA1(string input)
        {
            var output = string.Empty;
            var sha1 = new SHA1CryptoServiceProvider();
            var inputBytes = UTF8Encoding.UTF8.GetBytes(input);
            var outputBytes = sha1.ComputeHash(inputBytes);
            sha1.Clear();
            output = BitConverter.ToString(outputBytes);
            return output;
        }



        private String FormatTextXML(String fromUserName, String toUserName, String content)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return "<xml><ToUserName><![CDATA[" + fromUserName + "]]></ToUserName><FromUserName><![CDATA[" + toUserName + "]]></FromUserName><CreateTime>" + (DateTime.Now - startTime).TotalSeconds + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + content + "]]></Content><FuncFlag>1</FuncFlag></xml>";
        }


        // POST api/values
        public HttpResponseMessage Post(string ToUserName, string FromUserName, string CreateTime, string MsgType,string Content, string MsgId)
        {
            string xmlstr = FormatTextXML(FromUserName, ToUserName, Content);
            var result = new StringContent(xmlstr, UTF8Encoding.UTF8, "application/xml charset=\"utf-8\"");
            var response = new HttpResponseMessage { Content = result };
            return response;
        }
    }
}
