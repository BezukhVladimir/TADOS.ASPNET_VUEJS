namespace Content.Controllers.Rating.GetList
{
    using Models;
    using System.Collections.Generic;

    public class RatingGetListResponse
    {
        public IEnumerable<RatingIds> Ratings { get; set; }
    }
}