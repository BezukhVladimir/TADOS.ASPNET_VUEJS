namespace Content.Controllers.City.GetList
{
    using Models;
    using System.Collections.Generic;

    public class CityGetListResponse
    {
        public IEnumerable<City> Cities { get; set; }
    }
}