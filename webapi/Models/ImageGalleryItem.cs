namespace Content.Models
{
    public class ImageGalleryItem : ContentItem
    {
        public ImageItem Cover { get; set; }

        public IEnumerable<ImageItem> Images { get; set; }
    }
}