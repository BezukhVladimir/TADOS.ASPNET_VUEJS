namespace Content.Controllers.Image.Add
{
    using System.ComponentModel.DataAnnotations;

    public class ImageAddRequest
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string ContentName { get; set; }

        [Required]
        public string ContentCategory { get; set; }
        
        [Required]
        public string ImageURL { get; set; }
    }
}