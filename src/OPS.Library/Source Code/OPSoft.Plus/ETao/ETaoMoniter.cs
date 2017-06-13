using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Plugin.ETao
{
    public delegate void ETaoItemHandler(Item item);

    public class ETaoMoniter
    {
        public static event ETaoItemHandler OnItemUpload;

        internal ETaoMoniter()
        {
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="item"></param>
        public void UploadItem(Item item)
        {
            if (OnItemUpload != null)
            {
                OnItemUpload(item);
            }
        }


        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="itemID"></param>
        public void DelItem(string itemID)
        {
            //
            //TODO: Code here
            //
        }

        /// <summary>
        /// 更新类目文件
        /// </summary>
        /// <param name="rootCate"></param>
        public void RenewCats(Cate[] rootCate)
        {
            //
            //TODO:Cate update
            //
        }
    }
}
