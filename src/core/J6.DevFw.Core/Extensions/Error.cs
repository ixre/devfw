using JR.DevFw.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 错误
    /// </summary>
    public class Error:Exception
    {
        /// <summary>
        /// 创建新的错误
        /// </summary>
        /// <param name="message"></param>
        public Error(String message) : base(message)
        {

        }

        /// <summary>
        /// 转换为消息，通过Data["details"]获取详细的错误堆栈
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static Message ToMessage(Error err)
        {
            if(err != null)
            {
                IDictionary<String, String> extra = new Dictionary<String, String>();
                extra.Add("details", err.StackTrace);
                return new Message(1, err.Message, extra);
            }
            return new Message();
        }

        /// <summary>
        /// 转换为输出为简单的JSON格式，包含err_code和err_msg 节点
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static String SimpleJson(Error err)
        {
            int code;
            String msg;
            if (err == null) {
                code = 0;
                msg = "";
            }
            else
            {
                code = 1;
                msg = err.Message;
            }
            String[] arr = new string[] { "{", String.Format("\"err_code\":\"{0}\",\"err_msg\":\"{1}\"", code, err), "}"};
            return String.Join("",arr);


        }
    }
}
