namespace Content.Controllers.User.GetList
{
    public class UserGetListRequest
    {
        public string? CountryName { get; set; }

        public string? CityName { get; set; }

        public string Search { get; set; }
    }
}