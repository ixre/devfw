//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name:TemplateUtility.cs
// Author:newmin
// Create:2013/09/05
//

using System.Collections.Generic;

namespace AtNet.DevFw.Template
{
    /// <summary>
    /// IDataContainer接口
    /// </summary>
    public interface IDataContrainer
    {
        /// <summary>
        /// 获取模板页缓存内容
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        string GetTemplatePageCacheContent(string templateID);

        /// <summary>
        /// 设置模板页缓存内容
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="content"></param>
        /// <param name="dependFileName"></param>
        void SetTemplatePageCacheContent(string templateID, string content, string dependFileName);

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void DefineVariable<T>(string key, T variable);

        /// <summary>
        /// 定义变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="variable"></param>
        void DefineVariable(string key, Variable variable);

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetVariable(string key);

        /// <summary>
        /// 获取自定义变量
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetDefineVariable();
    }
}