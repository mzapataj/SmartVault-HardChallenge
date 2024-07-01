using System.Collections.Generic;
using System.Data;

namespace SmartVault.Shared.Interfaces
{
    public interface IDbContextService
    {
        IDbTransaction BeginTransaction();
        void Close();
        void CreateDatabase(string DatabaseName);
        int CreateTable<T>() where T : class;
        IEnumerable<T> GetAll<T>(string sqlWhereExpression, object param = null, IDbTransaction transaction = null) where T : class;
        IEnumerable<T> GetAll<T>(object param = null, IDbTransaction transaction = null) where T : class;
        long CountAll<T>(object param = null, IDbTransaction transaction = null) where T : class;
        T Insert<T>(T entity) where T : class;
        void Open();
        T UpdateById<T>(T entity) where T : class;
    }
}