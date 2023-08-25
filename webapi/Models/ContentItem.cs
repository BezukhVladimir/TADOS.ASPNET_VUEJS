namespace Content.Models
{
    public abstract class ContentItem 
    {
        public long Id { get; set; }

        public Author Author { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }
    }
}
