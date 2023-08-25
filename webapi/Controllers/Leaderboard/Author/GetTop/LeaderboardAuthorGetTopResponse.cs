namespace Content.Controllers.Leaderboard.Author.GetTop
{
    public class TopAuthor
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public double AverageRate { get; set; }
    }

    public class LeaderboardAuthorGetTopResponse
    {
        public IEnumerable<TopAuthor> Authors { get; set; }
    }
}