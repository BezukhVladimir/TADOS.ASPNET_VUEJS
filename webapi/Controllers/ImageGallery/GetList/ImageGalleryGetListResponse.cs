namespace Content.Controllers.ImageGallery.GetList
{
    using Models;
    using System.Collections.Generic;

    public class ImageGalleryGetListResponse
    {
        public IEnumerable<ImageGalleryItem> ImageGalleries { get; set; }
    }
}