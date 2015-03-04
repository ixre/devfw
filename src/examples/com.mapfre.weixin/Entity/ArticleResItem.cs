
using Com.Plugin.Core;

namespace Com.Plugin.Entity
{
    public class ArticleResItem
    {
        private ArticleRes _art;

        public ArticleResItem()
        {
            
        }
        public ArticleResItem(ArticleRes art)
        {
            this._art = art;
        }
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string Pic { get; set; }
        public int Sort { get; set; }
        public bool Enabled { get; set; }

        public void SetArticle(ArticleRes art)
        {
            this._art = art;
        }

        public int Save()
        {
            return IocObject.WeixinRes.SaveArticleItem(this._art.Id, this);
        }
    }
}
