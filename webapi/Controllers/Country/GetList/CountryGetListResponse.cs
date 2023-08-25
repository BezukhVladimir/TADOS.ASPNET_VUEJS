namespace Content.Controllers.Country.GetList
{
    using Models;
    using System.Collections.Generic;

    public class CountryGetListResponse
    {
        public IEnumerable<Country> Countries { get; set; }
    }
}
