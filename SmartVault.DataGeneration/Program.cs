using Microsoft.Extensions.Configuration;
using SmartVault.Core.Settings;
using SmartVault.Shared.Interfaces;
using SmartVault.Shared.Sql;
using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using SmartVault.Infrastructure.Services;
using SmartVault.Shared.Data;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var settings = configuration.Get<Settings>();

            IDbConnection connection = new SqliteConnection(settings.DefaultConnection);
            IDbCommandService command = new DbCommandService(); 
            
            IDbContextService dbContextService = new SQLiteDbContextService(connection, command);
            IDataGenerationService dataGenerationService = new DataGenerationService(dbContextService, settings);
            
            Console.WriteLine("Data generation is running...");
            
            var result = dataGenerationService.SeedDatabase();
            
            Console.WriteLine($"AccountCount: {result.accountData}");            
            Console.WriteLine($"DocumentCount: {result.documentData}");            
            Console.WriteLine($"UserCount: {result.userData}");                        
        }
    }
}