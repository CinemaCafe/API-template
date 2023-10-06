using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using _UniTechService.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace _UniTechService.Services
{
    public class UniService : IUniService
    {
        private readonly IConfiguration _config;

        public UniService(IConfiguration config)
        {
            _config = config;
        }
        public static string _connectionStr;
        private bool _enableMiniprofiler = true;
        private int _connectionTimeout = 60;
        private int _connectionTimeout_MS2K = 180;

        /// <summary>
        /// Get DB connection string
        /// </summary>
        /// <param name="dBEum">fLoginDB Enum</param>
        /// <returns></returns>
        public string GetDbStr(fLoginDB loginDB)
        {
            return loginDB switch
            {
                fLoginDB.MS_2017 => _config.GetValue<string>("ConnectionStrings:MS_2017"),
                fLoginDB.MS_2008 => _config.GetValue<string>("ConnectionStrings:MS_2008"),
                fLoginDB.MS_2000 => _config.GetValue<string>("ConnectionStrings:MS_2000"),
                fLoginDB.MS_UTADM => _config.GetValue<string>("ConnectionStrings:MS_UTADM"),
                _ => _config.GetValue<string>("ConnectionStrings:Default"),
            };
        }

        /// <summary>
        /// Connect the database
        /// </summary>
        /// <param name="DbString">DB Connect string</param>
        /// <returns></returns>
        public IDbConnection GetDbConnection(string DbString)
        {

            IDbConnection dBConnection;

            if (DbString != "")
            {
                //_connectionStr = DbContextDic(DbString);
                _connectionStr = DbString;
            }

            // miniprofiler check
            if (!_enableMiniprofiler)
            {
                dBConnection = new SqlConnection(_connectionStr);
            }
            else
            {
                dBConnection = new ProfiledDbConnection(
                    new SqlConnection(_connectionStr), MiniProfiler.Current);
            }


            if (dBConnection.State != ConnectionState.Open)
            {

                dBConnection.Open();
            }

            return dBConnection;
        }

        /// <summary>
        /// Connect the database for MS-SQL2000
        /// </summary>
        /// <param name="loginDB"></param>
        /// <returns></returns>
        public OdbcConnection GetDbConnectionODBC(fLoginDB loginDB)
        {
            OdbcConnection dBConnection;

            _connectionStr = GetDbStr(loginDB);

            dBConnection = new OdbcConnection(_connectionStr);

            if (dBConnection.State != ConnectionState.Open)
            {
                dBConnection.Open();
            }

            return dBConnection;
        }

        /// <summary>
        /// Query result return to Model
        /// </summary>
        /// <param name="querySql">Query string</param>
        /// <param name="param">Query params, default set null</param>
        /// <param name="DbEnum">DB Enum fLoginDB.MS_2017</param>
        /// <param name="commandType"></param>
        /// <typeparam name="TReturn"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<TReturn>> SqlCmdModel<TReturn>(string querySql, object param, fLoginDB loginDB, CommandType commandType = CommandType.Text)
        {
            // for MS-SQL2000
            // 2000 資料庫參數需用 "?" 前後包。
            if (loginDB == fLoginDB.MS_2000 || loginDB == fLoginDB.MS_UTADM)
            {
                using (OdbcConnection con = GetDbConnectionODBC(loginDB))
                {
                    return await con.QueryAsync<TReturn>(querySql, param, null, _connectionTimeout_MS2K, commandType).ConfigureAwait(false);
                }
            }
            else
            {
                using (IDbConnection con = GetDbConnection(GetDbStr(loginDB)))
                {
                    return await con.QueryAsync<TReturn>(querySql, param, null, _connectionTimeout, commandType).ConfigureAwait(false);
                }
            }
        }


        /// <summary>
        /// For insert、update、delete
        /// </summary>
        /// <param name="querySql"></param>
        /// <param name="param"></param>
        /// <param name="loginDB"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> SqlExecAsync(string querySql, object param, fLoginDB loginDB, CommandType commandType = CommandType.Text)
        {
            string str = GetDbStr(loginDB);
            int stataus = 0;
            if (loginDB == fLoginDB.MS_2000 || loginDB == fLoginDB.MS_UTADM)
            {
                // for ODBC connection.
                OdbcConnection con = GetDbConnectionODBC(loginDB);
                try
                {
                    stataus = await con.ExecuteAsync(querySql, param);
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }

            }
            else
            {
                IDbConnection con = GetDbConnection(str);
                try
                {
                    stataus = await con.ExecuteAsync(querySql, param);
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }

            }

            return stataus;
        }

    }
}