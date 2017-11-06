using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Net.Security;

namespace HM.Eleven.QQPlugins.Helper
{
    public class WebConnectHelper
    {
        /// <summary>
        /// 通过post方式发送数据
        /// </summary>
        /// <param name="postString"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string postData(string postString, string url)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //string postString = "";
            byte[] postData = Encoding.Default.GetBytes(postString);
            byte[] responseData = client.UploadData(url, "POST", postData);
            string srcString = Encoding.UTF8.GetString(responseData);
            return srcString;
        }

        /// <summary>
        /// 通过GET方式发送数据
        /// </summary>
        /// <param name="Url">url</param>
        /// <param name="postDataStr">GET数据</param>
        /// <param name="cookie">GET容器</param>
        /// <returns></returns>
        public static string getDataWithCookie(string url, ref CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            request.Method = "GET";
            request.ContentType = "image/JPEG;";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 检查ssh验证结果是否接受的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        /// <summary>
        /// 发送https请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="charset"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static HttpWebResponse CreatePostHttpResponse(string url, Encoding charset, IDictionary<string, string> parameters = null)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求  
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;
            //如果需要POST数据     
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = charset.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 使用webclient下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="savepath"></param>
        //public static void saveFile(string url, string cookie, string savepath)
        //{
        //    //string imgPath = getPath(textBox4.Text) + imgname + ".jpg";
        //    WebClient client = new WebClient();
        //    client.Headers.Add("Cookie", cookie);
        //    try
        //    {
        //        byte[] bytes = client.DownloadData(url);
        //        using (MemoryStream ms = new MemoryStream(bytes))
        //        {
        //            Image image = System.Drawing.Image.FromStream(ms);
        //            image.Save(savepath);
        //            //this.pictureBox1.Image = image;
        //        }
        //    }
        //    catch
        //    {
        //        return;
        //    }

        //}


        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }

        public static string getResultFromLtp(string words)
        {
            string url = "http://api.ltp-cloud.com/analysis/?api_key=p6I9Q5O05HxSYyrKVXscqZanlBId5cENRW7FQJIb&text=" + UrlEncode(words) + "&pattern=dp&format=json";
            string res = getData(url, Encoding.UTF8);
            return res;
        }

        /// <summary>
        /// 用httpWebRequest方式访问一个url，获取页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string getData(string url, Encoding encoding = null)
        {
            
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (encoding == null) encoding = Encoding.Default;
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), encoding);
                string responseContent = streamReader.ReadToEnd();
                httpWebResponse.Close();
                streamReader.Close();
                return responseContent;
            }
            catch (Exception)
            {
                return "";
            }

        }
    }
}
