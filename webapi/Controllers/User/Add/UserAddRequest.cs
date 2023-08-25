namespace Content.Controllers.User.Add
{
    using System.ComponentModel.DataAnnotations;

    public class UserAddRequest
    {
        [Required]
        public long CountryId { get; set; }
        
        [Required]
        public string CountryName { get; set; }

        [Required]
        public long CityId { get; set; }

        [Required]
        public string CityName { get; set; }

        [Required]
        public string UserEmail { get; set; }
    }
}
