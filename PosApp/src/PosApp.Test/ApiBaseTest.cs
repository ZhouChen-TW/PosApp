using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Web.Http;
using Autofac;
using NHibernate.Util;

namespace PosApp.Test
{
    public class ApiBaseTest:IDisposable
    {
        readonly HttpConfiguration webApiConfig = new HttpConfiguration();
        readonly HttpServer server;
        readonly IList<HttpClient> httpClients = new List<HttpClient>();
        readonly ILifetimeScope myLifetimeScope;

        public ApiBaseTest()
        {
            var bootstrap =new Bootstrap();
            bootstrap.Initialize(webApiConfig);
            server = new HttpServer(webApiConfig);
            myLifetimeScope = bootstrap.CreateLifetimeScope();
            ResetDatabase();
        }

        protected HttpClient CreateHttpClient()
        {
            var client = new HttpClient(server)
            {
                BaseAddress = new Uri("http://haha.com")
            };
            httpClients.Add(client);
            return client;
        }

        static void ResetDatabase()
        {
            const string cleanupSql =
                "EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';" +
                "EXEC sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN ( " +
                "ISNULL(OBJECT_ID(''[dbo].[VersionInfo]''), 0)) DELETE FROM ?';" +
                "EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'";

            string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                ExecuteSql(connection, cleanupSql);
            }
        }

        static void ExecuteSql(SqlConnection connection, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        protected ILifetimeScope GetLifetimeScope()
        {
            return myLifetimeScope;
        }

        public void Dispose()
        {
            httpClients.ForEach(c=>c.Dispose());
            server.Dispose();
            webApiConfig.Dispose();
            myLifetimeScope.Dispose();
        }
    }
}