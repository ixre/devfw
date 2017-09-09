using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JR.DevFw.Utils
{
    /// <summary>
    /// 实用工具
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 加载字体
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FontFamily LoadFontFamily(String path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            int fontSize = (int)stream.Length;
            byte[] data = new byte[fontSize];
            stream.Read(data, 0, fontSize);
            PrivateFontCollection fc = new PrivateFontCollection();
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            fc.AddMemoryFont(pointer, fontSize);
            return fc.Families[0];
        }
    }
}
