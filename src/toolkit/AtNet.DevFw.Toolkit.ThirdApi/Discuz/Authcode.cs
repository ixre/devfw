using System;
using System.Text;

namespace AtNet.DevFw.Toolkit.ThirdApi.Discuz
{
    public enum DiscuzAuthcodeMode { Encode, Decode };


    public class Authcode
    {
        private static Encoding encoding = Encoding.GetEncoding(Request.UC_CHARSET);

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        private static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }

                if (startIndex > str.Length)
                {
                    return "";
                }
            }
            else
            {
                if (length < 0)
                {
                    return "";
                }
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        private static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = encoding.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }

        /// <summary>
        /// 用于 RC4 处理密码
        /// </summary>
        /// <param name="pass">密码字串</param>
        /// <param name="kLen">密钥长度，一般为 256</param>
        /// <returns></returns>
        private static Byte[] GetKey(Byte[] pass, Int32 kLen)
        {
            Byte[] mBox = new Byte[kLen];

            for (Int64 i = 0; i < kLen; i++)
            {
                mBox[i] = (Byte)i;
            }
            Int64 j = 0;
            for (Int64 i = 0; i < kLen; i++)
            {
                j = (j + mBox[i] + pass[i % pass.Length]) % kLen;
                Byte temp = mBox[i];
                mBox[i] = mBox[j];
                mBox[j] = temp;
            }
            return mBox;
        }

        /// <summary>
        /// 生成随机字符
        /// </summary>
        /// <param name="lens">随机字符长度</param>
        /// <returns>随机字符</returns>
        private static string RandomString(int lens)
        {
            char[] CharArray = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int clens = CharArray.Length;
            string sCode = "";
            Random random = new Random();
            for (int i = 0; i < lens; i++)
            {
                sCode += CharArray[random.Next(clens)];
            }
            return sCode;
        }

        /// <summary>
        /// 使用 authcode 方法对字符串加密
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="expiry">加密字串有效时间，单位是秒</param>
        /// <returns>加密结果</returns>
        public static string DiscuzAuthcodeEncode(string source, string key, int expiry)
        {
            return DiscuzAuthcode(source, key, DiscuzAuthcodeMode.Encode, expiry);

        }

        /// <summary>
        /// 使用 Discuz authcode 方法对字符串加密
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密结果</returns>
        public static string DiscuzAuthcodeEncode(string source, string key)
        {
            return DiscuzAuthcode(source, key, DiscuzAuthcodeMode.Encode, 0);

        }

        /// <summary>
        /// 使用 Discuz authcode 方法对字符串解密
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密结果</returns>
        public static string DiscuzAuthcodeDecode(string source, string key)
        {
            return DiscuzAuthcode(source, key, DiscuzAuthcodeMode.Decode, 0);

        }

        /// <summary>
        /// 使用 变形的 rc4 编码方法对字符串进行加密或者解密
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="operation">操作 加密还是解密</param>
        /// <param name="expiry">密文有效期, 加密时候有效， 单 位 秒，0 为永久有效</param>
        /// <returns>加密或者解密后的字符串</returns>
        private static string DiscuzAuthcode(string source, string key, DiscuzAuthcodeMode operation, int expiry)
        {
            if (source == null || key == null)
            {
                return "";
            }

            int ckey_length = 4;
            string keya, keyb, keyc, cryptkey, result;

            key = MD5(key);
            keya = MD5(CutString(key, 0, 16));
            keyb = MD5(CutString(key, 16, 16));
            keyc = ckey_length > 0 ? (operation == DiscuzAuthcodeMode.Decode ? CutString(source, 0, ckey_length) : RandomString(ckey_length)) : "";

            cryptkey = keya + MD5(keya + keyc);

            if (operation == DiscuzAuthcodeMode.Decode)
            {
                byte[] temp;
                try
                {
                    temp = System.Convert.FromBase64String(CutString(source, ckey_length));
                }
                catch
                {
                    try
                    {
                        temp = System.Convert.FromBase64String(CutString(source + "=", ckey_length));
                    }
                    catch
                    {
                        try
                        {
                            temp = System.Convert.FromBase64String(CutString(source + "==", ckey_length));
                        }
                        catch
                        {
                            return "";
                        }
                    }
                }

                result = encoding.GetString(RC4(temp, cryptkey));

                //throw new Exception(CutString(result, 0, 10));
                long timestamp = long.Parse(CutString(result, 0, 10));

                if ((timestamp == 0 || timestamp - UnixTimestamp() > 0) && CutString(result, 10, 16) == CutString(MD5(CutString(result, 26) + keyb), 0, 16))
                {
                    return CutString(result, 26);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                source = (expiry == 0 ? "0000000000" : (expiry + UnixTimestamp()).ToString()) + CutString(MD5(source + keyb), 0, 16) + source;
                byte[] temp = RC4(encoding.GetBytes(source), cryptkey);
                return keyc + System.Convert.ToBase64String(temp);
            }
        }

        /// <summary>
        /// RC4 原始算法
        /// </summary>
        /// <param name="input">原始字串数组</param>
        /// <param name="pass">密钥</param>
        /// <returns>处理后的字串数组</returns>
        private static Byte[] RC4(Byte[] input, String pass)
        {
            if (input == null || pass == null) return null;

            byte[] output = new Byte[input.Length];
            byte[] mBox = GetKey(encoding.GetBytes(pass), 256);

            // 加密
            Int64 i = 0;
            Int64 j = 0;
            for (Int64 offset = 0; offset < input.Length; offset++)
            {
                i = (i + 1) % mBox.Length;
                j = (j + mBox[i]) % mBox.Length;
                Byte temp = mBox[i];
                mBox[i] = mBox[j];
                mBox[j] = temp;
                Byte a = input[offset];
                //Byte b = mBox[(mBox[i] + mBox[j] % mBox.Length) % mBox.Length];
                // mBox[j] 一定比 mBox.Length 小，不需要在取模
                Byte b = mBox[(mBox[i] + mBox[j]) % mBox.Length];
                output[offset] = (Byte)((Int32)a ^ (Int32)b);
            }

            return output;
        }


        private static string AscArr2Str(byte[] b)
        {
            return System.Text.UnicodeEncoding.Unicode.GetString(
             System.Text.ASCIIEncoding.Convert(System.Text.Encoding.ASCII,
             System.Text.Encoding.Unicode, b)
             );
        }

        public static long UnixTimestamp()
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtNow = DateTime.Parse(DateTime.Now.ToString());
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            return long.Parse(timeStamp.Substring(0, timeStamp.Length - 7));
        }

        public static string urlencode(string str)
        {
            //php的urlencode不同于HttpUtility.UrlEncode
            //return HttpUtility.UrlEncode(str);

            string tmp = string.Empty;
            string strSpecial = "_-.1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < str.Length; i++)
            {
                string crt = str.Substring(i, 1);
                if (strSpecial.Contains(crt))
                    tmp += crt;
                else
                {
                    byte[] bts = encoding.GetBytes(crt);
                    foreach (byte bt in bts)
                    {
                        tmp += "%" + bt.ToString("X");
                    }
                }
            }
            return tmp;
        }

        public static long time()
        {
            TimeSpan ts = new TimeSpan(System.DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }
    }
}