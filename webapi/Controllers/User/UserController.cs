namespace Content.Controllers.User
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
    [Route("api/content/user")]
    public class UserController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getList")]
        [ProducesResponseType(typeof(UserGetListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(UserGetListRequest request)
        {
            List<User> users = new List<User>();

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            List<string> countriesConditions = new List<string>();
            List<string> citiesConditions = new List<string>();
            List<string> usersConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.CountryName))
            {
                string[] keywords = request.CountryName.Split(' ');

                foreach (var key in keywords)
                {
                    countriesConditions.Add($"cntr.Name LIKE '%' || @{key} || '%'");
                    command.Parameters.AddWithValue(key, key);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.CityName))
            {
                string[] keywords = request.CityName.Split(' ');

                foreach (var key in keywords)
                {
                    citiesConditions.Add($"c.Name LIKE '%' || @{key} || '%'");
                    command.Parameters.AddWithValue(key, key);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                string[] keywords = request.Search.Split(' ');

                foreach (var key in keywords)
                {
                    usersConditions.Add($"u.Email LIKE '%' || @{key} || '%'");
                    command.Parameters.AddWithValue(key, key);
                }
            }

            string condition;
            if (countriesConditions.Count > 0) {
                if (citiesConditions.Count > 0) {
                    if (usersConditions.Count > 0) {
                        condition =
                            $@"{$"WHERE ({string.Join(" OR ", countriesConditions)}) AND ({string.Join(" OR ", citiesConditions)}) AND ({string.Join(" OR ", usersConditions)})"}";
                    } else {
                        condition =
                            $@"{$"WHERE ({string.Join(" OR ", countriesConditions)}) AND ({string.Join(" OR ", citiesConditions)})"}";
                    }
                } else {
                    if (usersConditions.Count > 0) {
                        condition =
                            $@"{$"WHERE ({string.Join(" OR ", countriesConditions)}) AND ({string.Join(" OR ", usersConditions)})"}";
                    } else  {
                        condition =
                            $@"{$"WHERE {string.Join(" OR ", countriesConditions)}"}";
                    }
                }
            } else {
                if (citiesConditions.Count > 0) {
                    if (usersConditions.Count > 0) {
                        condition =
                            $@"{$"WHERE ({string.Join(" OR ", citiesConditions)}) AND ({string.Join(" OR ", usersConditions)})"}";
                    } else {
                        condition =
                            $@"{$"WHERE {string.Join(" OR ", citiesConditions)}"}";
                    }
                } else {
                    if (usersConditions.Count > 0) {
                        condition =
                            $@"{$"WHERE {string.Join(" OR ", usersConditions)}"}";
                    } else {
                        condition = "";
                    }
                }
            }

            command.CommandText = $@"
                SELECT cntr.Id   CntrId,
                       cntr.Name CntrName,
                       c.Id      CId,
                       c.Name    CName,
                       u.Id      UId,
                       u.Email   UEmail
                  FROM User u
                  JOIN City c       ON c.Id    = u.CityId
                  JOIN Country cntr ON cntr.Id = c.CountryId
                 {condition}
                 ORDER BY cntr.Name, c.Name, u.Email ASC";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                User user = CreateUserFromReader(reader);
                users.Add(user);
            }

            await transaction.CommitAsync();

            UserGetListResponse response = new UserGetListResponse()
            {
                Users = users
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("get")]
        [ProducesResponseType(typeof(UserGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(UserGetRequest request)
        {
            User user = null;

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            command.CommandText = @"
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

            command.Parameters.AddWithValue("Id", request.Id);

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                user ??= CreateUserFromReader(reader);
            }

            await transaction.CommitAsync();

            UserGetResponse response = new UserGetResponse()
            {
                User = user
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(UserAddResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(UserAddRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkUserCommand = connection.CreateCommand();

            request.CountryName = request.CountryName?.Trim();
            request.CityName    = request.CityName?.Trim();
            request.UserEmail   = request.UserEmail?.Trim();

            checkUserCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM User u
                  JOIN City c       ON c.Id    = u.CityId
                  JOIN Country cntr ON cntr.Id = c.CountryId
                 WHERE cntr.Name = @CountryName
                       AND c.Name = @CityName
                       AND u.Email = @UserEmail";

            checkUserCommand.Parameters.AddWithValue("CountryName", request.CountryName);
            checkUserCommand.Parameters.AddWithValue("CityName", request.CityName);
            checkUserCommand.Parameters.AddWithValue("UserEmail", request.UserEmail);

            long currentCount = (long) checkUserCommand.ExecuteScalar();

            if (currentCount > 0)
                throw new Exception(
                    $"User with CountryName {request.CountryName} and with CityName {request.CityName} and with UserEmail {request.UserEmail} already exists");

            await using SQLiteCommand insertUserCommand = connection.CreateCommand();

            insertUserCommand.CommandText = @"
                INSERT INTO User(CityId, Email)
                VALUES (@CityId, @UserEmail);

                SELECT last_insert_rowid()";

            insertUserCommand.Parameters.AddWithValue("CityId", request.CityId);
            insertUserCommand.Parameters.AddWithValue("UserEmail", request.UserEmail);

            long userId = (long) insertUserCommand.ExecuteScalar();

            await transaction.CommitAsync();

            UserAddResponse response = new UserAddResponse()
            {
                Id = userId,
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(UserDeleteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(UserDeleteRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkUserCommand = connection.CreateCommand();

            checkUserCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM User u
                 WHERE u.Id = @Id;";

            checkUserCommand.Parameters.AddWithValue("Id", request.Id);

            long currentCount = (long) checkUserCommand.ExecuteScalar();

            if (currentCount == 0)
                throw new Exception($"User with id {request.Id} doesn't exist");

            await using (var setForeignKeysCommand = connection.CreateCommand())
            {
                setForeignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
                setForeignKeysCommand.ExecuteNonQuery();
            }

            await using SQLiteCommand deleteUserCommand = connection.CreateCommand();

            deleteUserCommand.CommandText = @"
                DELETE FROM User
                 WHERE Id = @Id;";

            deleteUserCommand.Parameters.AddWithValue("Id", request.Id);

            deleteUserCommand.ExecuteNonQuery();

            await transaction.CommitAsync();

            UserDeleteResponse response = new UserDeleteResponse()
            {
                Id = request.Id
            };

            connection.Close();
            return Json(response);
        }

        private static User CreateUserFromReader(DbDataReader reader)
        {
            long   countryId   = reader.GetInt64("CntrId");
            string countryName = reader.GetString("CntrName");
            long   cityId      = reader.GetInt64("CId");
            string cityName    = reader.GetString("CName");
            long   userId      = reader.GetInt64("UId");
            string userEmail   = reader.GetString("UEmail");

            User user = new User()
            {
                CountryId   = countryId,
                CountryName = countryName,
                CityId      = cityId,
                CityName    = cityName,
                UserId      = userId,
                UserEmail   = userEmail
            };

            return user;
        }
    }
}