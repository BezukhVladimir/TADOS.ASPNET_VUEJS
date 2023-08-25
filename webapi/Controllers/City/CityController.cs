namespace Content.Controllers.City
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
    [Route("api/content/city")]
    public class CityController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [HttpPost]
        [Route("getList")]
        [ProducesResponseType(typeof(CityGetListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(CityGetListRequest request)
        {
            List<City> cities = new List<City>();

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            List<string> countriesConditions = new List<string>();
            List<string> citiesConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                string[] keywords = request.Search.Split(' ');

                foreach (var key in keywords)
                {
                    citiesConditions.Add($"c.Name LIKE '%' || @{key} || '%'");
                    command.Parameters.AddWithValue(key, key);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.CountryName))
            {
                string[] keywords = request.CountryName.Split(' ');

                foreach (var key in keywords)
                {
                    countriesConditions.Add($"cntr.Name LIKE '%' || @{key} || '%'");
                    command.Parameters.AddWithValue(key, key);
                }
            }

            string condition;
            if (countriesConditions.Count > 0) {
                if (citiesConditions.Count > 0) {
                    condition =
                        $@"{$"WHERE ({string.Join(" OR ", countriesConditions)}) AND ({string.Join(" OR ", citiesConditions)})"}";
                } else {
                    condition = $@"{$"WHERE {string.Join(" OR ", countriesConditions)}"}";
                }
            } else {
                if (citiesConditions.Count > 0) {
                    condition = $@"{$"WHERE {string.Join(" OR ", citiesConditions)}"}";
                } else {
                    condition = "";
                }
            }

            command.CommandText = $@"
                SELECT cntr.Id   CntrId,
                       cntr.Name CntrName,
                       c.Id      CId,
                       c.Name    CName
                  FROM City c
                  JOIN Country cntr ON cntr.Id = c.CountryId
                 {condition}
                 ORDER BY cntr.Name, c.Name ASC";

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                City city = CreateCityFromReader(reader);
                cities.Add(city);
            }

            await transaction.CommitAsync();

            CityGetListResponse response = new CityGetListResponse()
            {
                Cities = cities
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("get")]
        [ProducesResponseType(typeof(CityGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(CityGetRequest request)
        {
            City city = null;

            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand command = connection.CreateCommand();

            command.CommandText = @"
                SELECT cntr.Id   CntrId,
                       cntr.Name CntrName,
                       c.Id      CId,
                       c.Name    CName
                  FROM City c
                  JOIN Country cntr ON cntr.Id = c.CountryId
                 WHERE c.Id = @Id";

            command.Parameters.AddWithValue("Id", request.Id);

            await using DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                city ??= CreateCityFromReader(reader);
            }

            await transaction.CommitAsync();

            CityGetResponse response = new CityGetResponse()
            {
                City = city
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(CityAddResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(CityAddRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkCityCommand = connection.CreateCommand();

            request.CountryName = request.CountryName?.Trim();
            request.CityName = request.CityName?.Trim();

            checkCityCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM City c
                  JOIN Country cntr ON cntr.Id = c.CountryId
                 WHERE cntr.Name = @CountryName
                       AND c.Name = @CityName;";

            checkCityCommand.Parameters.AddWithValue("CountryName", request.CountryName);
            checkCityCommand.Parameters.AddWithValue("CityName", request.CityName);

            long currentCount = (long) checkCityCommand.ExecuteScalar();

            if (currentCount > 0)
                throw new Exception(
                    $"City with CountryName {request.CountryName} and with CityName {request.CityName} already exists");

            await using SQLiteCommand insertCityCommand = connection.CreateCommand();

            insertCityCommand.CommandText = @"
                INSERT INTO City(CountryId, Name)
                VALUES (@CountryId, @CityName);

                SELECT last_insert_rowid()";

            insertCityCommand.Parameters.AddWithValue("CountryId", request.CountryId);
            insertCityCommand.Parameters.AddWithValue("CityName", request.CityName);

            long cityId = (long) insertCityCommand.ExecuteScalar();

            await transaction.CommitAsync();

            CityAddResponse response = new CityAddResponse()
            {
                Id = cityId,
            };

            connection.Close();
            return Json(response);
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(CityDeleteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(CityDeleteRequest request)
        {
            await using SQLiteConnection connection = new SQLiteConnection(DatabaseProvider.ConnectionString);
            await connection.OpenAsync();

            await using DbTransaction transaction =
                await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await using SQLiteCommand checkCityCommand = connection.CreateCommand();

            checkCityCommand.CommandText = @"
                SELECT COUNT(1)
                  FROM City c
                 WHERE c.Id = @Id;";

            checkCityCommand.Parameters.AddWithValue("Id", request.Id);

            long currentCount = (long) checkCityCommand.ExecuteScalar();

            if (currentCount == 0)
                throw new Exception($"City with id {request.Id} doesn't exist");

            await using (var setForeignKeysCommand = connection.CreateCommand())
            {
                setForeignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
                setForeignKeysCommand.ExecuteNonQuery();
            }

            await using SQLiteCommand deleteCityCommand = connection.CreateCommand();

            deleteCityCommand.CommandText = @"
                DELETE FROM City
                 WHERE Id = @Id;";

            deleteCityCommand.Parameters.AddWithValue("Id", request.Id);

            deleteCityCommand.ExecuteNonQuery();

            await transaction.CommitAsync();

            CityDeleteResponse response = new CityDeleteResponse()
            {
                Id = request.Id
            };

            connection.Close();
            return Json(response);
        }

        private static City CreateCityFromReader(DbDataReader reader)
        {
            long countryId     = reader.GetInt64("CntrId");
            string countryName = reader.GetString("CntrName");
            long cityId        = reader.GetInt64("CId");
            string cityName    = reader.GetString("CName");

            City city = new City()
            {
                CountryId   = countryId,
                CountryName = countryName,
                CityId      = cityId,
                CityName    = cityName
            };

            return city;
        }
    }
}