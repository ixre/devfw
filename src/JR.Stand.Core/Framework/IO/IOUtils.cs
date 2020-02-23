using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JR.DevFw.Framework.IO
{
    /// <summary>
    /// IO工具类
    /// </summary>
    public class IOUtils
    {
        /// <summary>
        /// 拷贝文件夹内容到目标文件夹
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="destFolder"></param>
        public static void CopyFolder(string srcFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder).Create();
            }
            foreach (var item in Directory.EnumerateFiles(srcFolder))
            {
                File.Copy(item, Path.Combine(destFolder, Path.GetFileName(item)), true);
            }
            foreach (var item in Directory.EnumerateDirectories(srcFolder))
            {
                CopyFolder(item, Path.Combine(destFolder, Path.GetFileName(item)));
            }
        }
    }
}
