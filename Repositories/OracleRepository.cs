using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace axiosTest.Repositories
{
    public class OracleRepository
    {
        private readonly string _connectionString;

        public OracleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        private OracleConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            return await conn.QueryAsync<T>(sql, param);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            return await conn.ExecuteAsync(sql, param);
        }
    }
}
