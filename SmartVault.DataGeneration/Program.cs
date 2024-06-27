using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartVault.Library;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            SQLiteConnection.CreateFile(configuration["DatabaseFileName"]);
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");
            var documentPath = new FileInfo("TestDoc.txt").FullName;

            var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");            

            using (var connection = new SQLiteConnection(string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"])))
            {
                CreateTables(files, connection);
                
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var documentNumber = 0;

                    var userInsert = @"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) 
                    VALUES ($index, 'FName' || $index, 'LName' || $index,$randomDate,$index,
                    'UserName-' || $index,'e10adc3949ba59abbe56e057f20f883e');

                    INSERT INTO Account (Id, Name) VALUES($index,'Account' || $index);";

                    var command = connection.CreateCommand();                    

                    var indexParam = command.CreateParameter();
                    indexParam.ParameterName = "$index";
                    command.Parameters.Add(indexParam);
                    
                    var randomDateParam = command.CreateParameter();
                    randomDateParam.ParameterName = "$randomDate";
                    command.Parameters.Add(randomDateParam);

                    var documentNumberParam = command.CreateParameter();
                    documentNumberParam.ParameterName = "documentNumber";
                    var documentNameParam = command.CreateParameter();
                    documentNameParam.ParameterName = "$documentName";
                    var documentPathParam = command.CreateParameter();
                    documentPathParam.ParameterName = "$documentPath";
                    var documentLengthParam = command.CreateParameter();
                    documentLengthParam.ParameterName = "$documentLength";

                    command.Parameters.Add(documentNumberParam);
                    command.Parameters.Add(documentNameParam);
                    command.Parameters.Add(documentPathParam);
                    command.Parameters.Add(documentLengthParam);

                    for (int i = 0; i < 100; i++)
                    {
                        var randomDayIterator = RandomDay().GetEnumerator();
                        randomDayIterator.MoveNext();
                        command.CommandText = userInsert;

                        indexParam.Value = i;
                        randomDateParam.Value = randomDayIterator.Current.ToString("yyyy-MM-dd");
                        
                        command.ExecuteNonQuery();

                        command.CommandText = $"INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES ($documentNumber,$documentName,$documentPath,$documentLength,$index);";

                        for (int d = 0; d < 10000; d++, documentNumber++)
                        {
                            documentNumberParam.Value = documentNumber;
                            documentNameParam.Value = $"Document{i}-{d}.txt";
                            documentPathParam.Value = documentPath;
                            documentLengthParam.Value = new FileInfo(documentPath).Length;

                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }

                var accountData = connection.Query("SELECT COUNT(*) FROM Account;");
                Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
                var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
                Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
                var userData = connection.Query("SELECT COUNT(*) FROM User;");
                Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");

                connection.Close();
            }
        }

        private static void CreateTables(string[] files, SQLiteConnection connection)
        {
            foreach (var file in files)
            {
                var serializer = new XmlSerializer(typeof(BusinessObject));
                var businessObject = serializer.Deserialize(new StreamReader(file)) as BusinessObject;
                connection.Execute(businessObject?.Script);
            }
        }

        static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}