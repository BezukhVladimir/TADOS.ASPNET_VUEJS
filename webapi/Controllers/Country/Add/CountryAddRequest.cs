namespace Content.Controllers.Country.Add
{
    using System.ComponentModel.DataAnnotations;

    public class CountryAddRequest
    {
        [Required]
        public string CountryName { get; set; }
    }
}
