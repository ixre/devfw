/*
 * Copyright 2010 OPS,All right reseved .
 * name     : ftpclient
 * author   : newmin
 * date     : 2010/12/13
 */

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace JR.DevFw.Framework.Net
{
    public class HttpClient
    {
        private string uri;

        private HttpClient(string uri)
        {
            this.uri = uri;
        }

        public static string Post(string url, string postData, CookieCollection cookies)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null) return null;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null) return null;
            }

            //request.Accept = "*/*";
            request.Method = "POST";
            //httpwebrequest控件有一个透明过程，先向服务方查询url是否存在而不发送POST的内容，
            //服务器如果证实url是可访问的，才发送POST，早期的Apache就认为这是一种错误，而IIS却可以正确应答
            //加上下面这一句将查询服务后马上post数据
            //System.Net.ServicePointManager.Expect100Continue = false;
            //Expect:100-continue
            System.Net.ServicePointManager.Expect100Continue = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 10000;

            //添加cookie
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            //发送请求
            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Dispose();
            //获取响应
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            else
            {
                return "Error:" + response.StatusCode.ToString();
            }
        }

        public string Post(string postData, CookieCollection cookies)
        {
            return Post(this.uri, postData, cookies);
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //总是接受  
            return true;
        }


        /// <summary>
        /// 下载文件（支持断点续传）并返回字节数组
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileBytes">上次接收的文件</param>
        /// <returns></returns>
        public static byte[] DownloadFile(string url, byte[] fileBytes)
        {
            const int buffer = 32768; //32k
            byte[] data = new byte[buffer];
            int cread;
            int cTotal;

            MemoryStream ms = fileBytes == null || fileBytes.Length == 0
                ? new MemoryStream()
                : new MemoryStream(fileBytes);

            string remoteAddr = url;

            int fileLength = (int) ms.Length;

            HttpWebRequest wr = WebRequest.Create(remoteAddr) as HttpWebRequest;
            if (fileLength != 0)
            {
                wr.AddRange(fileLength);
            }

            try
            {
                WebResponse rsp = wr.GetResponse();

                Stream st = rsp.GetResponseStream();

                cTotal = (int) rsp.ContentLength;

                while ((cread = st.Read(data, 0, buffer)) != 0)
                {
                    ms.Write(data, 0, cread);
                }

                byte[] streamArray = ms.ToArray();
                ms.Dispose();

                return streamArray;
            }
            catch
            {
            }
            return null;
        }

        //
        //TODO:做一个支持断电下载的方法
        //
    }
}