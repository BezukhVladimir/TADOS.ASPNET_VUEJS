namespace Content.Controllers.Leaderboard.Content
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using GetTop;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Providers;

    [ApiController]
    [Route("api/content/leaderboard/content")]
    public class LeaderboardContentController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getTop")]
        [ProducesResponseType(typeof(LeaderboardContentGetTopResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTop(LeaderboardContentGetTopRequest request)
        {
            List<TopContent> topContents = new List<TopContent>();

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();
            
            command.CommandText = $@"
                SELECT
                    CI.Id                    AS ContentItemId,
                    CI.Name                  AS ContentName,
                    CI.Category              AS ContentCategory,
                    COALESCE(AVG(R.Rate), 0) AS AverageRate
                FROM
                    ContentItem CI
                LEFT JOIN
                    Rating R ON CI.Id = R.RatedContentItemId
                GROUP BY
                    CI.Id, CI.Name, CI.Category
                ORDER BY
                    AverageRate DESC
                LIMIT { request.Count }
                ";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                TopContent topContent = CreateTopContentFromReader(reader);
                topContents.Add(topContent);
            }

            await transaction.CommitAsync();

            LeaderboardContentGetTopResponse response = new LeaderboardContentGetTopResponse()
            {
                Contents = topContents
            };

            connection.Close();
            return Json(response);
        }

        private static TopContent CreateTopContentFromReader(DbDataReader reader)
        {
            long   id          = reader.GetInt64("ContentItemId");
            string name        = reader.GetString("ContentName");
            string category    = reader.GetString("ContentCategory");
            double averageRate = reader.GetDouble("AverageRate");

            TopContent topContent = new TopContent()
            {
                Id          = id,
                Name        = name,
                Category    = category,
                AverageRate = averageRate
            };

            return topContent;
        }
    }
}