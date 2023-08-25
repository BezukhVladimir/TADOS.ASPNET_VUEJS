namespace Content.Controllers.Rating
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Add;
    using Delete;
    using Get;
    using GetList;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Providers;

    [ApiController]
    [Route("api/content/rating")]
    public class RatingController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getList")]
        [ProducesResponseType(typeof(RatingGetListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(RatingGetListRequest request)
        {
            List<RatingIds> ratings = new List<RatingIds>();

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();
            
            command.CommandText = $@"
                SELECT r.Id                 RId,
                       r.RatedContentItemId RRatedContentItemId,
                       r.RaterId            RRater,
                       r.Rate               RRate
                  FROM Rating r";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                RatingIds rating = CreateRatingFromReader(reader);
                ratings.Add(rating);
            }

            await transaction.CommitAsync();

            RatingGetListResponse response = new RatingGetListResponse()
            {
                Ratings = ratings
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("get")]
        [ProducesResponseType(typeof(RatingGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(RatingGetRequest request)
        {
            RatingIds ratingIds = null;

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            command.CommandText = $@"
                SELECT r.Id                 RId,
                       r.RatedContentItemId RRatedContentItemId,
                       r.RaterId            RRater,
                       r.Rate               RRate
                  FROM Rating r
                 WHERE r.Id = @Id";

            command.Parameters.AddWithValue("Id", request.Id);

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ratingIds ??= CreateRatingFromReader(reader);
            }

            await transaction.CommitAsync();

            RatingGetResponse response = new RatingGetResponse()
            {
                Rating = ratingIds
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(RatingAddResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(RatingAddRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkRaterCommand = connection.CreateCommand();

            checkRaterCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM Rater r
                 WHERE r.UserId = @Id;";

            checkRaterCommand.Parameters.AddWithValue("Id", request.RaterId);

            long currentCount = (long) checkRaterCommand.ExecuteScalar();

            if (currentCount == 0)
            {
                await using SQLiteCommand createRaterCommand = connection.CreateCommand();

                createRaterCommand.CommandText = @"
                    INSERT INTO Rater(UserId)
                    VALUES (@UserId)";

                createRaterCommand.Parameters.AddWithValue("UserId", request.RaterId);
                createRaterCommand.ExecuteScalar();
            }

            await using (var setForeignKeysCommand = connection.CreateCommand())
            {
                setForeignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
                setForeignKeysCommand.ExecuteNonQuery();
            }

            await using SQLiteCommand insertRatingCommand = connection.CreateCommand();

            insertRatingCommand.CommandText = @"
                INSERT INTO Rating(RatedContentItemId, RaterId, Rate)
                VALUES (@RatedContentItemId, @RaterId, @Rate);

                SELECT last_insert_rowid()";

            insertRatingCommand.Parameters.AddWithValue("RatedContentItemId", request.RatedId);
            insertRatingCommand.Parameters.AddWithValue("RaterId", request.RaterId);
            insertRatingCommand.Parameters.AddWithValue("Rate", request.Rate);

            long RatingId = (long) insertRatingCommand.ExecuteScalar();

            await transaction.CommitAsync();

            RatingAddResponse response = new RatingAddResponse()
            {
                Id = RatingId,
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(RatingDeleteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(RatingDeleteRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkRatingCommand = connection.CreateCommand();

            checkRatingCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM Rating r
                 WHERE r.Id = @Id;";

            checkRatingCommand.Parameters.AddWithValue("Id", request.Id);

            long currentCount = (long) checkRatingCommand.ExecuteScalar();

            if (currentCount == 0)
                throw new Exception($"Rating with id {request.Id} doesn't exist");

            await using (var setForeignKeysCommand = connection.CreateCommand())
            {
                setForeignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
                setForeignKeysCommand.ExecuteNonQuery();
            }

            await using SQLiteCommand deleteRatingCommand = connection.CreateCommand();

            deleteRatingCommand.CommandText = @"
                DELETE FROM Rating
                 WHERE Id = @Id;";

            deleteRatingCommand.Parameters.AddWithValue("Id", request.Id);

            deleteRatingCommand.ExecuteNonQuery();

            await transaction.CommitAsync();

            RatingDeleteResponse response = new RatingDeleteResponse()
            {
                Id = request.Id
            };

            connection.Close();
            return Json(response);
        }

        private static RatingIds CreateRatingFromReader(DbDataReader reader)
        {
            long ratingId           = reader.GetInt64("RId");
            long ratedContentItemId = reader.GetInt64("RRatedContentItemId");
            long raterId            = reader.GetInt64("RRaterId");
            long rate               = reader.GetInt64("RRate");
            
            RatingIds rating = new RatingIds()
            {
                Id      = ratingId,
                RatedId = ratedContentItemId,
                RaterId = raterId,
                Rate    = (Rate) rate
            };

            return rating;
        }
    }
}