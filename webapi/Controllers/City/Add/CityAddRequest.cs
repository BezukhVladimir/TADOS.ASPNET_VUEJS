namespace Content.Controllers.City.Add
{
    using System.ComponentModel.DataAnnotations;

    public class CityAddRequest
    {
        [Required]
        public long CountryId { get; set; }

        [Required]
        public string CountryName { get; set; }

        [Required]
        public string CityName { get; set; }
    }
}
