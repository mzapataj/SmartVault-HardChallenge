using Dapper;
using SmartVault.Shared.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SmartVault.Shared.Sql
{
    public class SQLiteDbContextService : IDbContextService
    {
        private IDbConnection connection;
        private IDbCommandService commandService;
     
        public SQLiteDbContextService(IDbConnection connection, IDbCommandService commandService)
        {
            this.connection = connection;
            this.commandService = commandService;
        }
        public void CreateDatabase(string DatabaseName)
        {
            SQLiteConnection.CreateFile(DatabaseName);
        }

        public int CreateTable<T>() where T : class
        {
            return commandService.CreateTable<T>();
        }
        public T Insert<T>(T entity) where T : class
        {
            commandService.Insert(entity);
            return entity;
        }
        public T UpdateById<T>(T entity) where T : class
        {
            commandService.UpdateById(entity);
            return entity;
        }

        public long CountAll<T>(object param = null, IDbTransaction transaction = null) where T : class
        {            
            return (long) connection.ExecuteScalar($"SELECT COUNT(*) FROM [{typeof(T).Name}];", param, transaction);
        }

        public IEnumerable<T> GetAll<T>(object param = null, IDbTransaction transaction = null) where T : class
        {
            return connection.Query<T>($"SELECT * FROM [{typeof(T).Name}];", param, transaction);
        }

        public IEnumerable<T> GetAll<T>(string sqlWhereExp, object param = null, IDbTransaction transaction = null) where T : class
        {
            return connection.Query<T>($"SELECT * FROM [{typeof(T).Name}] WHERE {sqlWhereExp};", param, transaction);
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = connection.BeginTransaction();
            commandService.Init(connection.CreateCommand());
            return transaction;
        }

        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            commandService?.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
