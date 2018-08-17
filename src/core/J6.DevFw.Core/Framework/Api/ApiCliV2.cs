using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Security.Cryptography;

namespace Com.Line.Api.Sdk
{
    /// <summary>
    ///  接口响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        // 接口执行状态码
        public int Code;
        // 接口返回消息
        public String Message;
        // 接口返回数据
        public T Data;

    }

    public class ApiCliV2
    {
        private string _key = "";
        private string _secret = "";
        private string _signType = "sha1";
        private string _apiUrl = "";
        private int _timeout = 5000000;

        /// <summary>
        /// 配置接口
        /// </summary>
        /// <param name="apiServer"></param>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="signType"></param>
        /// <param name="timeout">请求超时时间，单位毫秒</param>
        public ApiCliV2(string apiServer, string key, string secret, string signType,int timeout)
        {
            this._apiUrl = apiServer;
            this._key = key;
            this._secret = secret;
            this._signType = signType;
            this._timeout = timeout;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public String Sign(IDictionary<string, string> dict, String secret)
        {
            // 参数排序
            SortedDictionary<string, string> sortDict = new SortedDictionary<string, string>();
            foreach (var d in dict)
            {
                if (d.Key != "sign" && d.Key != "sign_type")
                {
                    sortDict.Add(d.Key, d.Value);
                }
            }
            // 拼接字符
            var sb = new StringBuilder();
            int i = 0;
            foreach (var d in sortDict)
            {
                if (i > 0)
                {
                    sb.Append("&");
                }
                sb.Append(d.Key);
                sb.Append("=");
                sb.Append(d.Value);
                i++;
            }
            return EncryptHelper.Sha1(sb.ToString() + secret).ToLower();
        }

        private string parseParams(IDictionary<string, string> dict)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<String, String> p in dict)
            {
                if (i++ > 0) sb.Append("&");
                sb.Append(p.Key).Append("=").Append(HttpUtility.UrlEncode(p.Value));
            }
            return sb.ToString();
        }

        private String Request(String api, IDictionary<string, string> dict, int timeout)
        {
            if (_apiUrl == "" || _key == "" || _signType == "" || _secret == "")
            {
                throw new ArgumentException("请先调用Configure()方法初始化接口");
            }
            // 附加参数
            dict.Add("api", api);
            dict.Add("key", _key);
            dict.Add("sign_type", _signType);
            String sign = Sign(dict, _secret);
            dict.Add("sign", sign);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_apiUrl);
            request.Method = "POST";
            request.KeepAlive = false;
            ServicePointManager.Expect100Continue = false;
            if (timeout > 0)
            {
                request.Timeout = timeout;
            }
            request.ProtocolVersion = HttpVersion.Version10;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:58.0) Gecko/20100101 Firefox/58.0";
            request.ContentType = "application/x-www-form-urlencoded";

            //发送请求
            byte[] data = Encoding.UTF8.GetBytes(parseParams(dict));
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Dispose();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                String body = reader.ReadToEnd();
                reader.Close();
                return body;
            }
            else
            {
                throw new Exception("远程服务器" + response.StatusDescription);
            }
        }

        public String Post(String api, IDictionary<string, string> dict)
        {
            return this.Post(api, dict, this._timeout);
        }

        public String Post(String api, IDictionary<string, string> dict,int timeout)
        {
            return Request(api, dict,timeout);
        }

        public string Call(String api, IDictionary<string, string> param)
        {
            return this.Call(api, param, this._timeout);
        }
        public string Call(String api,IDictionary<string,string> param,int timeout)
        {
            String ret = this.Request(api, param,timeout);
            if (ret.StartsWith("#"))
            {
                throw this.Except(ret);
            }
            Response<Object> rsp = JsonConvert.DeserializeObject<Response<Object>>(ret);
            return JsonConvert.SerializeObject(rsp.Data);
        }

        private string GetExceptionMessage<T>(Response<T> rs)
        {
            return String.Format("ErrCode:{0} ; ErrMsg:{1}", rs.Code, rs.Message);
        }

        public T SinglePost<T>(String api, IDictionary<string, string> dict)
        {
            return this.SinglePost<T>(api, dict, this._timeout);
        }

        public T SinglePost<T>(String api, IDictionary<string, string> dict, int timeout)
        {
            String rspText = this.Request(api, dict,timeout);
            if (rspText.StartsWith("#"))
            {
                throw this.Except(rspText);
            }
            Response<T> rsp = JsonConvert.DeserializeObject<Response<T>>(rspText);
            return rsp.Data;
        }

        private Exception Except(string rspText)
        {
            return new Exception(rspText.Substring(1));
        }

        public T[] MultiPost<T>(String api, IDictionary<string, string> dict)
        {
            return this.MultiPost<T>(api, dict);
        }
        public T[] MultiPost<T>(String api, IDictionary<string, string> dict, int timeout)
        {
            String rspText = this.Request(api, dict,timeout);
            if (rspText.StartsWith("#"))
            {
                throw this.Except(rspText);
            }
            Response<T>[] rsp = JsonConvert.DeserializeObject<Response<T>[]>(rspText);

            T[] arr = new T[rsp.Length];
            for(int i =0;i<rsp.Length;i++){
                arr[i] = rsp[i].Data;
            }
            return arr;
        }



    }

    internal static class EncryptHelper
    {

        /// <summary>
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Md5(this string str)
        {
            //将输入字符串转换成字节数组
            var buffer = Encoding.Default.GetBytes(str);
            //接着，创建Md5对象进行散列计算
            var data = MD5.Create().ComputeHash(buffer);

            //创建一个新的Stringbuilder收集字节
            var sb = new StringBuilder();

            //遍历每个字节的散列数据 
            foreach (var t in data)
            {
                //格式每一个十六进制字符串
                sb.Append(t.ToString("X2"));
            }

            //返回十六进制字符串
            return sb.ToString();
        }

        /// <summary>
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Sha1(this string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            var data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="stream">要计算哈希值的 Stream</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] HashData(System.IO.Stream stream, string algName)
        {
            System.Security.Cryptography.HashAlgorithm algorithm;
            if (algName == null)
            {
                throw new ArgumentNullException("algName 不能为 null");
            }
            if (string.Compare(algName, "sha1", true) == 0)
            {
                algorithm = System.Security.Cryptography.SHA1.Create();
            }
            else
            {
                if (string.Compare(algName, "md5", true) != 0)
                {
                    throw new Exception("algName 只能使用 sha1 或 md5");
                }
                algorithm = System.Security.Cryptography.MD5.Create();
            }
            return algorithm.ComputeHash(stream);
        }

        /// <summary>
        /// 字节数组转换为16进制表示的字符串
        /// </summary>
        private static string ByteArrayToHexString(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "");
        }
    }
}
