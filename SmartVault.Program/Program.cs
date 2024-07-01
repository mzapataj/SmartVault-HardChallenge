using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SmartVault.Core.Settings;
using SmartVault.Infrastructure.Services;
using SmartVault.Shared.Data;
using SmartVault.Shared.Interfaces;
using SmartVault.Shared.Sql;

namespace SmartVault.Program
{
    partial class Program
    {
        private static IDocumentService documentService;
        private static IDbContextService dbContextService;
        private static IDbConnection connection;
        private static IDbCommandService command;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var settings = configuration.Get<Settings>();

            connection = new SqliteConnection(settings.DefaultConnection);
            command = new DbCommandService(); 
            dbContextService = new SQLiteDbContextService(connection, command);
            documentService = new DocumentService(dbContextService);
            
            WriteEveryThirdFileToFile(args[0]);
            GetAllFileSizes();
        }

        private static void GetAllFileSizes()
        {
            Console.WriteLine("GetAllFileSizes method is running...");
            var count = documentService.GetAllFileSizes();
            Console.WriteLine($"{count} files were updated");
        }

        private static void WriteEveryThirdFileToFile(string accountId)
        {
            Console.WriteLine("WriteEveryThirdFileToFile method is running...");
            var count = documentService.WriteEveryThirdFileToFile(accountId);
            Console.WriteLine($"{count} files contains '{documentService.TextToMatch}' text");
        }
    }
}