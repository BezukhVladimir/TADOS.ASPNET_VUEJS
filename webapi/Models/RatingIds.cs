namespace Content.Models
{
    public class RatingIds
    {
        public long Id { get; set; }

        public long RatedId { get; set; }

        public long RaterId { get; set; }

        public Rate Rate { get; set; }
    }
}