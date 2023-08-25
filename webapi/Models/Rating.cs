namespace Content.Models
{
    public class Rating
    {
        public long Id { get; set; }

        public ContentItem Rated { get; set; }

        public User Rater { get; set; }

        public Rate Rate { get; set; }
    }
}