namespace Content.Models
{
    public class User : City
    {
        public long UserId { get; set; }

        public string UserEmail { get; set; }
    }
}
