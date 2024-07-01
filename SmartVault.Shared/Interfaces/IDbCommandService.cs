using System;
using System.Data;

namespace SmartVault.Shared.Interfaces
{
    public interface IDbCommandService : IDisposable
    {
        void Init(IDbCommand command);
        int CreateTable<T>() where T : class;
        void Insert<T>(T entity) where T : class;
        void UpdateById<T>(T entity) where T : class;
    }
}