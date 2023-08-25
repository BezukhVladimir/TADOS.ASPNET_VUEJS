namespace Content.Controllers.Image.GetList
{
    using Models;
    using System.Collections.Generic;

    public class ImageGetListResponse
    {
        public IEnumerable<ImageItem> Images { get; set; }
    }
}