namespace Content.Controllers.Country
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Add;
    using Get;
    using GetList;
    using Delete;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Providers;

    [ApiController]
    [Route("api/content/country")]
    public class CountryController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getList")]
        [ProducesResponseType(typeof(CountryGetListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(CountryGetListRequest request)
        {
            List<Country> countries = new List<Country>();

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            
            await using SQLiteCommand command = connection.CreateCommand();

            List<string> conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                conditions.Add("c.Name LIKE '%' || @Search || '%'");
                command.Parameters.AddWithValue("Search", request.Search);
            }

            command.CommandText = $@"
                SELECT c.Id   CId,
                       c.Name CName
                  FROM Country c
                {(conditions.Count > 0 ? $"WHERE {string.Join(" AND ", conditions)}" : string.Empty)}
                 ORDER BY c.Name DESC";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Country country = CreateCountryFromReader(reader);
                countries.Add(country);
            }

            await transaction.CommitAsync();

            CountryGetListResponse response = new CountryGetListResponse()
            {
                Countries = countries
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("get")]
        [ProducesResponseType(typeof(CountryGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(CountryGetRequest request)
        {
            Country country = null;
            
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            command.CommandText = @"
                SELECT c.Id   CId,
                       c.Name CName
                  FROM Country c
                 WHERE c.Id = @Id";

            command.Parameters.AddWithValue("Id", request.Id);

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                country ??= CreateCountryFromReader(reader);
            }

            await transaction.CommitAsync();

            CountryGetResponse response = new CountryGetResponse()
            {
                Country = country
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(CountryAddResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(CountryAddRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            
            await using SQLiteCommand checkCountryCommand = connection.CreateCommand();

            request.CountryName = request.CountryName?.Trim();

            checkCountryCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM Country c
                 WHERE c.Name = @Name;";

            checkCountryCommand.Parameters.AddWithValue("Name", request.CountryName);

            long currentCount = (long) checkCountryCommand.ExecuteScalar();

            if (currentCount > 0)
                throw new Exception($"Country with name {request.CountryName} already exists");

            await using SQLiteCommand insertCountryCommand = connection.CreateCommand();

            insertCountryCommand.CommandText = @"
                PRAGMA foreign_keys = ON;

                INSERT INTO Country(Name)
                VALUES (@Name);
                
                SELECT last_insert_rowid()";

            insertCountryCommand.Parameters.AddWithValue("Name", request.CountryName);

            long countryId = (long) insertCountryCommand.ExecuteScalar();

            await transaction.CommitAsync();

            CountryAddResponse response = new CountryAddResponse()
            {
                Id = countryId
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(CountryDeleteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(CountryDeleteRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkCountryCommand = connection.CreateCommand();

            checkCountryCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM Country c
                 WHERE c.Id = @Id;";

            checkCountryCommand.Parameters.AddWithValue("Id", request.Id);

            long currentCount = (long) checkCountryCommand.ExecuteScalar();

            if (currentCount == 0)
                throw new Exception($"Country with id {request.Id} doesn't exist");

            await using (var setForeignKeysCommand = connection.CreateCommand())
            {
                setForeignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
                setForeignKeysCommand.ExecuteNonQuery();
            }

            await using SQLiteCommand deleteCountryCommand = connection.CreateCommand();

            deleteCountryCommand.CommandText = @"
                DELETE FROM Country
                 WHERE Id = @Id;";

            deleteCountryCommand.Parameters.AddWithValue("Id", request.Id);

            deleteCountryCommand.ExecuteNonQuery();

            await transaction.CommitAsync();

            CountryDeleteResponse response = new CountryDeleteResponse()
            {
                Id = request.Id
            };

            connection.Close();
            return Json(response);
        }

        private static Country CreateCountryFromReader(DbDataReader reader)
        {
            long   id   = reader.GetInt64("CId");
            string name = reader.GetString("CName");

            Country country = new Country()
            {
                CountryId   = id,
                CountryName = name
            };

            return country;
        }
    }
}