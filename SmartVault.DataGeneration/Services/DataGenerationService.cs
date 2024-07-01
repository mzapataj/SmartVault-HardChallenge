using Newtonsoft.Json;
using SmartVault.Core.BusinessObjects;
using SmartVault.Core.Settings;
using SmartVault.DataGeneration.Dto;
using SmartVault.DataGeneration.Interfaces;
using SmartVault.Shared.Interfaces;
using System;
using System.IO;


using DateTimeExtensions = SmartVault.Shared.Utils.DateTimeExtensions;

namespace SmartVault.DataGeneration.Services
{
    public class DataGenerationService : IDataGenerationService
    {
        private readonly ISettings settings;
        private IDbContextService context;

        public DataGenerationService(IDbContextService context, ISettings settings)
        {
            this.context = context;
            this.settings = settings;            
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
            var documentPath = new FileInfo("TestDoc.txt").FullName;

            for (int i = 0; i < records; i++)
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
            for (int d = 0; d < records; d++, documentNumber++)
            {
                var document = new Document()
                {
                    Id = documentNumber,
                    Name = $"Document{userId}-{d}.txt",
                    FilePath = documentPath,
                    Length = (int)new FileInfo(documentPath).Length,
                    CreatedAt = DateTime.Now
                };

                context.Insert(document);
            }

            return documentNumber;
        }

        public DataGenerationResult SeedDatabase()
        {
            string accountData, documentData, userData;
            context.CreateDatabase(settings.DatabaseFileName);
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");
            
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
