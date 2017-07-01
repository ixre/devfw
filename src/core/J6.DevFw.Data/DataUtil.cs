using System;
using System.Data.Common;
using System.Text;

namespace JR.DevFw.Data
{
    public class DataUtil
    {
        /// <summary>
        /// 转换为参数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DbParameter[] ToParams(IDataBase db, object[,] data)
        {
            if (data != null)
            {
                int l = data.GetLength(0);
                if (l != 0 && data.GetLength(1) != 2)
                {
                    throw new ArgumentOutOfRangeException("data", "多纬数组的二维长度必须为2");
                }

                DbParameter[] parameter = new DbParameter[l];
                for (int i = 0; i < l; i++)
                {
                    parameter[i] = db.CreateParameter(data[i, 0].ToString(), data[i, 1]);
                }
                return parameter;
            }
            return null;
        }

        /// <summary>
        /// 参数转为字符
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static String ParamsToString(DbParameter[] parameters)
        {
            if (parameters != null)
            {
                int l = parameters.Length;
                if (l > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i=0;i<l;i++){
                        if (i > 0) sb.Append(" ");
                        sb.Append(parameters[i].ParameterName).Append(":")
                                .Append(parameters[i].Value.ToString());
                    }
                    return sb.ToString();
                }
            }
            return "";
        } 
    }
}
