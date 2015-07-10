/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : IocObject.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using AtNet.DevFw.Data;
using Com.Plugin.Core.ILogic;
using Com.Plugin.Core.Logic;
using Com.Plugin.ILogic;
using Com.Plugin.Logic;
using StructureMap;

namespace Com.Plugin.Core
{
    /// <summary>
    /// Description of LogicHelper.
    /// </summary>
    public class IocObject
    {
        static IocObject()
        {
            ObjectFactory.Configure(o =>
            {
                o.For<DbGenerator>().Singleton().Use(new DbGenerator());
                o.For<IDataLogic>().Singleton().Use<DataLogic>();
                o.For<IWeixinResLogic>().Singleton().Use<WeixinResLogic>();
            }
                );

            Data = ObjectFactory.GetInstance<IDataLogic>();
           WeixinRes = ObjectFactory.GetInstance<IWeixinResLogic>();
            _dao = ObjectFactory.GetInstance<DbGenerator>();
        }

        public static readonly IWeixinResLogic WeixinRes;

        internal static readonly IDataLogic Data;
        private static DbGenerator _dao;

        public static DataBaseAccess GetDao()
        {
            return _dao.New();
        }
    }
}
