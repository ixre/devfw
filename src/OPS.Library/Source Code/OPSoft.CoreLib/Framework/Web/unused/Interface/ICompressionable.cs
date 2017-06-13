using System.Web.UI;

namespace Ops.Framework.Web.unused.Interface
{
    /// <summary>
    /// 压缩网页接口
    /// </summary>
    public interface ICompressionable
    {
        void Compression(HtmlTextWriter writer);
    }
}