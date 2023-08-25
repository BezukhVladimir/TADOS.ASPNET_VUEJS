namespace Content.Controllers.Video.Add
{
    using System.ComponentModel.DataAnnotations;

    public class VideoAddRequest
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string ContentName { get; set; }

        [Required]
        public string ContentCategory { get; set; }
        
        [Required]
        public string VideoURL { get; set; }
    }
}