using System;
using System.Collections.Generic;

namespace JR.DevFw.Framework
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 错误码,0表示成功
        /// </summary>
        public int errCode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public String errMsg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public IDictionary<String, String> data { get; set; }

        /// <summary>
        /// 创建新的消息
        /// </summary>
        public Message()
        {

        }
        /// <summary>
        /// 创建新的消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public Message(int code, String msg, IDictionary<String, String> data)
        {
            this.errCode = code;
            this.errMsg = msg;
            this.data = data;
        }
        /// <summary>
        /// 创建新的消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public static Message New(int code, String msg, IDictionary<String, String> data)
        {
            Message m = new Message();
            m.errCode = code;
            m.errMsg = msg;
            m.data = data;
            return m;
        }
    }
}
