namespace Content.Controllers.User.GetList
{
    using Models;
    using System.Collections.Generic;

    public class UserGetListResponse
    {
        public IEnumerable<User> Users { get; set; }
    }
}