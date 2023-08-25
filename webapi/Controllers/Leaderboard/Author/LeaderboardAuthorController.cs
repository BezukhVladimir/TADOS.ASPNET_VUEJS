namespace Content.Controllers.Leaderboard.Author
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
    [Route("api/content/leaderboard/author")]
    public class LeaderboardAuthorController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getTop")]
        [ProducesResponseType(typeof(LeaderboardAuthorGetTopResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTop(LeaderboardAuthorGetTopRequest request)
        {
            List<TopAuthor> topAuthors = new List<TopAuthor>();

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            command.CommandText = $@"
                WITH ContentAvgRating AS (
                    SELECT
                        C.AuthorId  AS UserId,
                        C.Id        AS ContentId,
                        AVG(R.Rate) AS AvgContentRating
                    FROM
                        ContentItem C
                    LEFT JOIN
                        Rating R ON C.Id = R.RatedContentItemId
                    GROUP BY
                        C.AuthorId, C.Id
                ),
                UserAverageRatings AS (
                    SELECT
                        A.UserId                  AS UserId,
                        AVG(CAR.AvgContentRating) AS AvgUserRating
                    FROM
                        Author A
                    LEFT JOIN
                        ContentAvgRating CAR ON A.UserId = CAR.UserId
                    GROUP BY
                        A.UserId
                )
                SELECT
                    U.Id                           AS Id,
                    U.Email                        AS Email,
                    COALESCE(UAR.AvgUserRating, 0) AS AverageRate
                FROM
                    User U
                LEFT JOIN
                    UserAverageRatings UAR ON U.Id = UAR.UserId
                ORDER BY
                    AverageRate DESC
                LIMIT {request.Count}
                ";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                TopAuthor topAuthor = CreateTopAuthorFromReader(reader);
                topAuthors.Add(topAuthor);
            }

            await transaction.CommitAsync();

            LeaderboardAuthorGetTopResponse response = new LeaderboardAuthorGetTopResponse()
            {
                Authors = topAuthors
            };

            connection.Close();
            return Json(response);
        }

        private static TopAuthor CreateTopAuthorFromReader(DbDataReader reader)
        {
            long   id          = reader.GetInt64("Id");
            string email       = reader.GetString("Email");
            double averageRate = reader.GetDouble("AverageRate");

            TopAuthor topAuthor = new TopAuthor()
            {
                Id          = id,
                Email       = email,
                AverageRate = averageRate
            };

            return topAuthor;
        }
    }
}