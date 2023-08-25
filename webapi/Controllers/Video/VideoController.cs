namespace Content.Controllers.Video
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
    [Route("api/content/video")]
    public class VideoController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getList")]
        [ProducesResponseType(typeof(VideoGetListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(VideoGetListRequest request)
        {
            List<VideoItem> videos = new List<VideoItem>();

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();
            
            List<string> contentNameConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.ContentName))
            {
                string[] keywords = request.ContentName.Split(' ');

                foreach (var key in keywords)
                {
                    contentNameConditions.Add($"cnt.Name LIKE '%' || @{key} || '%'");
                    command.Parameters.AddWithValue(key, key);
                }
            }

            string condition;
            if (contentNameConditions.Count > 0) {
                condition = $@"{$"WHERE {string.Join(" OR ", contentNameConditions)}"}";
            } else {
                condition = "";
            }
            
                
            command.CommandText = $@"
                SELECT cnt.Id       CntId,
                       cnt.AuthorId CntAuthorId,
                       cnt.Name     CntName,
                       cnt.Category CntCategory
                  FROM ContentItem cnt
                  JOIN VideoItem v ON v.ContentItemId = cnt.Id
                  {condition}
                 ORDER BY cnt.Name ASC";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                VideoItem video = CreateVideoItemFromReader(connection, reader);
                videos.Add(video);
            }

            await transaction.CommitAsync();

            VideoGetListResponse response = new VideoGetListResponse()
            {
                Videos = videos
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("get")]
        [ProducesResponseType(typeof(VideoGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(VideoGetRequest request)
        {
            VideoItem video = null;

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            command.CommandText = $@"
                SELECT cnt.Id       CntId,
                       cnt.AuthorId CntAuthorId,
                       cnt.Name     CntName,
                       cnt.Category CntCategory
                  FROM ContentItem cnt
                  JOIN VideoItem v ON v.ContentItemId = cnt.Id
                 WHERE cnt.Id = @Id";

            command.Parameters.AddWithValue("Id", request.Id);

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                video??= CreateVideoItemFromReader(connection, reader);
            }

            await transaction.CommitAsync();

            VideoGetResponse response = new VideoGetResponse()
            {
                Video = video
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(VideoAddResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(VideoAddRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkAuthorCommand = connection.CreateCommand();

            checkAuthorCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM Author a
                 WHERE a.UserId = @Id;";

            checkAuthorCommand.Parameters.AddWithValue("Id", request.UserId);

            long currentCount = (long) checkAuthorCommand.ExecuteScalar();

            if (currentCount == 0)
            {
                await using SQLiteCommand createAuthorCommand = connection.CreateCommand();

                createAuthorCommand.CommandText = @"
                    INSERT INTO Author(UserId)
                    VALUES (@UserId)";

                createAuthorCommand.Parameters.AddWithValue("UserId", request.UserId);
                createAuthorCommand.ExecuteScalar();
            }

            await using SQLiteCommand insertContentItemCommand = connection.CreateCommand();

            insertContentItemCommand.CommandText = @"
                INSERT INTO ContentItem(AuthorId, Name, Category)
                VALUES (@UserId, @ContentName, @ContentCategory);

                SELECT last_insert_rowid()";

            insertContentItemCommand.Parameters.AddWithValue("UserId", request.UserId);
            insertContentItemCommand.Parameters.AddWithValue("ContentName", request.ContentName);
            insertContentItemCommand.Parameters.AddWithValue("ContentCategory", request.ContentCategory);

            long contentItemId = (long) insertContentItemCommand.ExecuteScalar();

            await using SQLiteCommand insertVideoItemCommand = connection.CreateCommand();

            insertVideoItemCommand.CommandText = @"
                INSERT INTO VideoItem(ContentItemId, URL)
                VALUES (@ContentItemId, @URL);";

            insertVideoItemCommand.Parameters.AddWithValue("ContentItemId", contentItemId);
            insertVideoItemCommand.Parameters.AddWithValue("URL", request.VideoURL);
            
            await insertVideoItemCommand.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            VideoAddResponse response = new VideoAddResponse()
            {
                Id = contentItemId,
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(VideoDeleteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(VideoDeleteRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkContentCommand = connection.CreateCommand();

            checkContentCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM ContentItem cnt
                  JOIN VideoItem v ON v.ContentItemId = cnt.Id
                 WHERE cnt.Id = @Id;";

            checkContentCommand.Parameters.AddWithValue("Id", request.Id);

            long currentCount = (long) checkContentCommand.ExecuteScalar();

            if (currentCount == 0)
                throw new Exception($"VideoItem with id {request.Id} doesn't exist");

            await using (var setForeignKeysCommand = connection.CreateCommand())
            {
                setForeignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
                setForeignKeysCommand.ExecuteNonQuery();
            }

            await using SQLiteCommand deleteContentCommand = connection.CreateCommand();

            deleteContentCommand.CommandText = @"
                DELETE FROM ContentItem
                 WHERE Id = @Id;";

            deleteContentCommand.Parameters.AddWithValue("Id", request.Id);

            deleteContentCommand.ExecuteNonQuery();

            await transaction.CommitAsync();

            VideoDeleteResponse response = new VideoDeleteResponse()
            {
                Id = request.Id
            };

            connection.Close();
            return Json(response);
        }

        private static Author CreateAuthorFromReader(DbDataReader reader)
        {
            long   countryId   = reader.GetInt64("CntrId");
            string countryName = reader.GetString("CntrName");
            long   cityId      = reader.GetInt64("CId");
            string cityName    = reader.GetString("CName");
            long   userId      = reader.GetInt64("UId");
            string userEmail   = reader.GetString("UEmail");

            Author author = new Author()
            {
                CountryId   = countryId,
                CountryName = countryName,
                CityId      = cityId,
                CityName    = cityName,
                UserId      = userId,
                UserEmail   = userEmail
            };

            return author;
        }

        private static VideoItem CreateVideoItemFromReader(SQLiteConnection connection, DbDataReader reader)
        {
            long   contentId       = reader.GetInt64("CntId");
            long   authorId        = reader.GetInt64("CntAuthorId");
            string contentName     = reader.GetString("CntName");
            string contentCategory = reader.GetString("CntCategory");

            SQLiteCommand getVideoURL = connection.CreateCommand();

            getVideoURL.CommandText = $@"
                SELECT v.URL VURL
                  FROM VideoItem v
                 WHERE v.ContentItemId = @ContentId";

            getVideoURL.Parameters.AddWithValue("ContentId", contentId);

            DbDataReader videoItemReader = getVideoURL.ExecuteReader();

            string videoURL = "";
            if (videoItemReader.Read())
            {
                videoURL = videoItemReader.GetString("VURL");
            }

            Author author = null; 

            SQLiteCommand getUser = connection.CreateCommand();
            
            getUser.CommandText = @"
                SELECT cntr.Id   CntrId,
                       cntr.Name CntrName,
                       c.Id      CId,
                       c.Name    CName,
                       u.Id      UId,
                       u.Email   UEmail
                  FROM User u
                  JOIN City c       ON c.Id    = u.CityId
                  JOIN Country cntr ON cntr.Id = c.CountryId
                 WHERE u.Id = @Id";

            getUser.Parameters.AddWithValue("Id", authorId);

            DbDataReader userReader = getUser.ExecuteReader();

            while (userReader.Read())
            {
                author ??= CreateAuthorFromReader(userReader);
            }

            VideoItem videoItem = new VideoItem()
            {
                Id       = contentId,
                Author   = author,
                Name     = contentName,
                Category = contentCategory,
                URL      = videoURL
            };

            return videoItem;
        }
    }
}