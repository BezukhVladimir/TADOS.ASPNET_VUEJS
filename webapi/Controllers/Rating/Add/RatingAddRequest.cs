namespace Content.Controllers.Rating.Add
{
    public class RatingAddRequest
    {
        public long RatedId { get; set; }

        public long RaterId { get; set; }

        public long Rate { get; set; }
    }
}