namespace Content.Controllers.Article.Add
{
    using System.ComponentModel.DataAnnotations;

    public class ArticleAddRequest
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string ContentName { get; set; }

        [Required]
        public string ContentCategory { get; set; }
        
        [Required]
        public string ArticleText { get; set; }
    }
}