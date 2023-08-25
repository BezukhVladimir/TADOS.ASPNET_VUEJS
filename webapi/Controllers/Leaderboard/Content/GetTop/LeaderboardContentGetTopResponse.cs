namespace Content.Controllers.Leaderboard.Content.GetTop
{
    public class TopContent
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public double AverageRate { get; set; }
    }

    public class LeaderboardContentGetTopResponse
    {
        public IEnumerable<TopContent> Contents { get; set; }
    }
}