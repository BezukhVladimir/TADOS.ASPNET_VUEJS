namespace Content.Controllers.Article.GetList
{
    using Models;
    using System.Collections.Generic;

    public class ArticleGetListResponse
    {
        public IEnumerable<ArticleItem> Articles { get; set; }
    }
}