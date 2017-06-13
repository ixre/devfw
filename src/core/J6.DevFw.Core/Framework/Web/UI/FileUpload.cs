/*
 * 文件上传
 * Copyright 2012 OPS,All right reseved!
 * Newmin(ops.cc)  @  2012-09-29 07:09
 * 
 */

using System;
using System.IO;
using System.Web;

namespace JR.DevFw.Framework.Web.UI
{
    /// <summary>
    /// 文件上传工具
    /// </summary>
    public class FileUpload
    {
        /// <summary>
        /// 保存文件夹
        /// </summary>
        private readonly string _saveAbsoluteDir;

        /// <summary>
        /// 文件名
        /// </summary>
        private readonly string _fileName;

        private UploadFileInfo _fileInfo;

        public FileUpload(string saveAbsoluteDir, string fileName)
        {
            this._saveAbsoluteDir = saveAbsoluteDir;
            this._fileName = fileName;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <returns>异步则返回进程ID，同步返回上传文件的路径</returns>
        public string Upload()
        {
            HttpRequest request = HttpContext.Current.Request;
            String baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] process = request.Form["upload_process"].Split('|');
            string processID = process[1],
                field = process[0];

            var postedFile = request.Files[field];
            if (postedFile == null)
            {
                return null;
            }
            string fileExt = postedFile.FileName.Substring(postedFile.
                FileName.LastIndexOf('.') + 1); //扩展名

            _fileInfo = new UploadFileInfo
            {
                Id = processID,
                ContentLength = postedFile.ContentLength,
                FilePath = String.Format("{0}{1}.{2}", this._saveAbsoluteDir, _fileName, fileExt)
            };

            InitUplDirectory(baseDir, this._saveAbsoluteDir);
            saveStream(postedFile, baseDir + _fileInfo.FilePath);

            return _fileInfo.FilePath;
        }

        private static void InitUplDirectory(String baseDir, String absDir)
        {
            //如果文件夹不存在，则创建文件夹
            String dir = baseDir + absDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir).Create();
            }
        }

        private void saveStream(HttpPostedFile postedFile, string path)
        {
            const int bufferSize = 100; //缓冲区大小
            byte[] buffer = new byte[bufferSize]; //缓冲区

            int bytes; //从流中读取的值
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                while (true)
                {
                    bytes = postedFile.InputStream.Read(buffer, 0, bufferSize);
                    if (bytes == 0)
                    {
                        break;
                    }
                    fs.Write(buffer, 0, bytes);
                }
                fs.Flush();
                fs.Close();
            }
        }
    }
}