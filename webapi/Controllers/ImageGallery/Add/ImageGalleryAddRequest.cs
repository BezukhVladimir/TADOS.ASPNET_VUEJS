namespace Content.Controllers.ImageGallery.Add
{
    using System.ComponentModel.DataAnnotations;

    public class ImageGalleryAddRequest
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string ContentName { get; set; }

        [Required]
        public string ContentCategory { get; set; }
        
        [Required]
        public long CoverImageId { get; set; }

        [Required]
        public IEnumerable<long> ImageIds { get; set; }
    }
}