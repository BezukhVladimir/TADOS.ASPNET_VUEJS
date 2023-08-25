namespace Content.Controllers.ImageGallery
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
    [Route("api/content/image_gallery")]
    public class ImageGalleryController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getList")]
        [ProducesResponseType(typeof(ImageGalleryGetListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(ImageGalleryGetListRequest request)
        {
            List<ImageGalleryItem> galleries = new List<ImageGalleryItem>();

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
                  JOIN ImageGalleryItem ig ON ig.ContentItemId = cnt.Id
                  {condition}
                 ORDER BY cnt.Name ASC";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ImageGalleryItem gallery = CreateImageGalleryItemFromReader(connection, reader);
                galleries.Add(gallery);
            }

            await transaction.CommitAsync();

            ImageGalleryGetListResponse response = new ImageGalleryGetListResponse()
            {
                ImageGalleries = galleries
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("get")]
        [ProducesResponseType(typeof(ImageGalleryGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(ImageGalleryGetRequest request)
        {
            ImageGalleryItem imageGallery = null;

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
                  JOIN ImageGalleryItem ig ON ig.ContentItemId = cnt.Id
                 WHERE cnt.Id = @Id";

            command.Parameters.AddWithValue("Id", request.Id);

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                imageGallery ??= CreateImageGalleryItemFromReader(connection, reader);
            }

            await transaction.CommitAsync();

            ImageGalleryGetResponse response = new ImageGalleryGetResponse()
            {
                ImageGallery = imageGallery
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(ImageGalleryAddResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(ImageGalleryAddRequest request)
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

            await using SQLiteCommand insertImageGalleryItemCommand = connection.CreateCommand();

            insertImageGalleryItemCommand.CommandText = @"
                INSERT INTO ImageGalleryItem(ContentItemId, CoverId)
                VALUES (@ContentItemId, @CoverId);";

            insertImageGalleryItemCommand.Parameters.AddWithValue("ContentItemId", contentItemId);
            insertImageGalleryItemCommand.Parameters.AddWithValue("CoverId", request.CoverImageId);
            
            await insertImageGalleryItemCommand.ExecuteNonQueryAsync();

            foreach (var imageId in request.ImageIds)
            {
                await using SQLiteCommand insertGalleryImageCommand = connection.CreateCommand();

                insertGalleryImageCommand.CommandText = @"
                INSERT INTO GalleryImage(ImageGalleryItemId, ImageItemId)
                VALUES (@ImageGalleryItemId, @ImageItemId);";

                insertGalleryImageCommand.Parameters.AddWithValue("ImageGalleryItemId", contentItemId);
                insertGalleryImageCommand.Parameters.AddWithValue("ImageItemId", imageId);

                await insertGalleryImageCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            ImageGalleryAddResponse response = new ImageGalleryAddResponse()
            {
                Id = contentItemId,
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(ImageGalleryDeleteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(ImageGalleryDeleteRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkContentCommand = connection.CreateCommand();

            checkContentCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM ContentItem cnt
                  JOIN ImageGalleryItem ig ON ig.ContentItemId = cnt.Id
                 WHERE cnt.Id = @Id;";

            checkContentCommand.Parameters.AddWithValue("Id", request.Id);

            long currentCount = (long) checkContentCommand.ExecuteScalar();

            if (currentCount == 0)
                throw new Exception($"ImageGalleryItem with id {request.Id} doesn't exist");

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

            ImageGalleryDeleteResponse response = new ImageGalleryDeleteResponse()
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

        private static ImageItem CreateImageItemFromReader(SQLiteConnection connection, DbDataReader reader)
        {
            long   contentId       = reader.GetInt64("CntId");
            long   authorId        = reader.GetInt64("CntAuthorId");
            string contentName     = reader.GetString("CntName");
            string contentCategory = reader.GetString("CntCategory");

            SQLiteCommand getImageURL = connection.CreateCommand();

            getImageURL.CommandText = $@"
                SELECT im.URL ImURL
                  FROM ImageItem im
                 WHERE im.ContentItemId = @ContentId";

            getImageURL.Parameters.AddWithValue("ContentId", contentId);

            DbDataReader imageItemReader = getImageURL.ExecuteReader();

            string imageURL = "";
            if (imageItemReader.Read())
            {
                imageURL = imageItemReader.GetString("ImURL");
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

            ImageItem imageItem = new ImageItem()
            {
                Id       = contentId,
                Author   = author,
                Name     = contentName,
                Category = contentCategory,
                URL      = imageURL
            };

            return imageItem;
        }

        private static ImageGalleryItem CreateImageGalleryItemFromReader(SQLiteConnection connection, DbDataReader reader)
        {
            long   contentId       = reader.GetInt64("CntId");
            long   authorId        = reader.GetInt64("CntAuthorId");
            string contentName     = reader.GetString("CntName");
            string contentCategory = reader.GetString("CntCategory");

            SQLiteCommand getImageGalleryCoverId = connection.CreateCommand();

            getImageGalleryCoverId.CommandText = $@"
                SELECT ig.CoverId IgCover
                  FROM ImageGalleryItem ig
                 WHERE ig.ContentItemId = @ContentId";

            getImageGalleryCoverId.Parameters.AddWithValue("ContentId", contentId);

            DbDataReader imageGalleryItemReader = getImageGalleryCoverId.ExecuteReader();

            long imageGalleryCoverId = 0;
            if (imageGalleryItemReader.Read())
            {
                imageGalleryCoverId = imageGalleryItemReader.GetInt64("IgCover");
            }

            SQLiteCommand getCover = connection.CreateCommand();

            getCover.CommandText = $@"
                SELECT cnt.Id       CntId,
                       cnt.AuthorId CntAuthorId,
                       cnt.Name     CntName,
                       cnt.Category CntCategory
                  FROM ContentItem cnt
                  JOIN ImageItem im ON im.ContentItemId = cnt.Id
                 WHERE cnt.Id = @Id";

            getCover.Parameters.AddWithValue("Id", imageGalleryCoverId);

            DbDataReader getCoverReader = getCover.ExecuteReader();

            ImageItem coverItem = null;
            while (getCoverReader.Read())
            {
                coverItem ??= CreateImageItemFromReader(connection, getCoverReader);
            }

            SQLiteCommand getImageIds = connection.CreateCommand();

            getImageIds.CommandText = $@"
                SELECT gIm.ImageItemId gImItem
                  FROM GalleryImage gIm
                 WHERE gIm.ImageGalleryItemId = @ContentId";

            getImageIds.Parameters.AddWithValue("ContentId", contentId);

            DbDataReader imageItemsReader = getImageIds.ExecuteReader();

            List<long> imageIds = new List<long>();
            while (imageItemsReader.Read())
            {
                imageIds.Add(imageItemsReader.GetInt64("gImItem"));
            }

            List<ImageItem> images = new List<ImageItem>();
            foreach (var imageId in imageIds)
            {
                SQLiteCommand getImage = connection.CreateCommand();

                getImage.CommandText = $@"
                SELECT cnt.Id       CntId,
                       cnt.AuthorId CntAuthorId,
                       cnt.Name     CntName,
                       cnt.Category CntCategory
                  FROM ContentItem cnt
                  JOIN ImageItem im ON im.ContentItemId = cnt.Id
                 WHERE cnt.Id = @Id";

                getImage.Parameters.AddWithValue("Id", imageId);

                DbDataReader getImageReader = getImage.ExecuteReader();

                ImageItem imageItem = null;
                while (getImageReader.Read())
                {
                    imageItem ??= CreateImageItemFromReader(connection, getImageReader);
                }

                images.Add(imageItem);
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

            ImageGalleryItem imageGalleryItem = new ImageGalleryItem()
            {
                Id       = contentId,
                Author   = author,
                Name     = contentName,
                Category = contentCategory,
                Cover    = coverItem,
                Images   = images
            };

            return imageGalleryItem;
        }
    }
}