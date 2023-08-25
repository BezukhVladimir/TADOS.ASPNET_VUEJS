namespace Content.Controllers.Video.GetList
{
    using Models;
    using System.Collections.Generic;

    public class VideoGetListResponse
    {
        public IEnumerable<VideoItem> Videos { get; set; }
    }
}