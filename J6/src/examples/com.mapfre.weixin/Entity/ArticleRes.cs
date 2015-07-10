using System;
using System.Collections.Generic;
using Com.Plugin.Core;

namespace Com.Plugin.Entity
{
   public  class ArticleRes:IWxRes
   {
       public int Id { get; set; }

       public string GetKey()
       {
           return this.ResKey;
       }
       public int Type()
       {
           return this.TypeId;
       }
       public string ResKey { get; set; }
       public int TypeId { get; set; }
       public DateTime CreateTime { get; set; }
       public DateTime UpdateTime { get; set; }
       public String TypeName { get; set; }

       public IList<ArticleResItem> Items { get; set; }


       public int Save()
       {
           return IocObject.WeixinRes.Save(this);
       }
   }
}
