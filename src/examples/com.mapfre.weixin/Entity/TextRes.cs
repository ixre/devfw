using System;
using Com.Plugin.Core;

namespace Com.Plugin.Entity
{
    public class TextRes:IWxRes
    {
        public int Id { get; set; }

        public string GetKey()
        {
            return this.ResKey;
        }
        public string ResKey { get; set; }
        public int TypeId { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public String TypeName { get; set; }


        public int Save()
        {
            return IocObject.WeixinRes.Save(this);
        }

        public int Type()
        {
            return this.TypeId;
        }
    }
}
