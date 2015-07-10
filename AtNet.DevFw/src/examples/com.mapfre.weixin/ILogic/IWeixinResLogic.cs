using Com.Plugin.Entity;

namespace Com.Plugin.ILogic
{
    public interface IWeixinResLogic
    {
        IWxRes GetResById(int id);
        bool CheckResKey(int id, string resKey);
        int Save(IWxRes res);
        ArticleResItem GetArticleItem(int id);
        int SaveArticleItem(int id, ArticleResItem item);
        IWxRes GetResByKey(string resKey);
        void DeleteRes(int resId);
    }
}
