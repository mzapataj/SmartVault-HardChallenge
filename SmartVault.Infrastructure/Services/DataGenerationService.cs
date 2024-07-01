using Newtonsoft.Json;
using SmartVault.Core.BusinessObjects;
using SmartVault.Core.Settings;
using SmartVault.Shared.Interfaces;
using System;
using System.IO;
using SmartVault.Infrastructure.Dto;
using DateTimeExtensions = SmartVault.Shared.Utils.DateTimeExtensions;

namespace SmartVault.Infrastructure.Services
{
    public interface IDataGenerationService
    {
        DataGenerationResult SeedDatabase();
    }
    public class DataGenerationService : IDataGenerationService
    {
        private readonly ISettings settings;
        private readonly IDbContextService context;
        private readonly string appDataPath;
        
        public DataGenerationService(IDbContextService context, ISettings settings)
        {
            this.context = context;
            this.settings = settings;
            
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), settings.AppName);
        }

        private void CreateTables()
        {
            context.CreateTable<Account>();
            context.CreateTable<Document>();
            context.CreateTable<User>();
        }

        private void InsertUsers(int records = 100)
        {
            var documentNumber = 0;
            var documentPath = Path.Combine(appDataPath, "TestDoc.txt");

            for (int i = 1; i <= records; i++)
            {
                var randomDayIterator = DateTimeExtensions.RandomDays().GetEnumerator();
                randomDayIterator.MoveNext();
                
                var user = new User()
                {
                    Id = i,
                    FirstName = "FName" + i,
                    LastName = "LName" + i,
                    DateOfBirth = randomDayIterator.Current,
                    AccountId = i,
                    Username = "UserName-" + i,
                    Password = "e10adc3949ba59abbe56e057f20f883e",
                    CreatedAt = DateTime.Now,
                };
                context.Insert(user);
                
                var account = new Account()
                {
                    Id = i,
                    Name = "Account" + i,
                    CreatedAt = DateTime.Now,
                };
                context.Insert(account);

                documentNumber = InsertDocuments(documentNumber, documentPath, i);
            }
        }

        private int InsertDocuments(int documentNumber, string documentPath, int userId, int records = 10_000)
        {
            for (int d = 1; d <= records; d++, documentNumber++)
            {
                var document = new Document()
                {
                    Id = documentNumber,
                    Name = $"Document{userId}-{d}.txt",
                    FilePath = documentPath,
                    Length = (int)new FileInfo(documentPath).Length,
                    CreatedAt = DateTime.Now,
                    AccountId = userId
                };

                context.Insert(document);
            }

            return documentNumber;
        }

        public DataGenerationResult SeedDatabase()
        {
            string accountData, documentData, userData;
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
            context.CreateDatabase(Path.Combine(appDataPath, settings.DatabaseFileName));
            File.WriteAllText(Path.Combine( appDataPath, "TestDoc.txt"), $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");
            
            context.Open();

            using (var transaction = context.BeginTransaction())
            {
                CreateTables();
                InsertUsers();
                transaction.Commit();
            }

            accountData = JsonConvert.SerializeObject(context.CountAll<Account>());
            documentData = JsonConvert.SerializeObject(context.CountAll<Document>());
            userData = JsonConvert.SerializeObject(context.CountAll<User>());

            context.Close();

            return new DataGenerationResult(accountData, documentData, userData);
        }
    }
}
