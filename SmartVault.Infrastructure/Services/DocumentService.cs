using SmartVault.Core.BusinessObjects;
using SmartVault.Core.Settings;
using SmartVault.Shared.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace SmartVault.Infrastructure.Services
{
    public interface IDocumentService : IDisposable
    {
        string TextToMatch { get; }
        long GetAllFileSizes();
        long WriteEveryThirdFileToFile(string accountId, TextWriter outputFile = null);
    }

    public class DocumentService : IDocumentService
    {
        private readonly IDbContextService dbContextService;
        public string TextToMatch => "Smith Property";


        public DocumentService(IDbContextService dbContextService)
        {
            this.dbContextService = dbContextService;            
            dbContextService.Open();
        }

        public long GetAllFileSizes()
        {
            long updatedFilesCount = 0;
            var documents = dbContextService.GetAll<Document>();

            foreach (var document in documents)
            {
                var filePath = document.FilePath;
                var fileSize = (int)new FileInfo(filePath).Length;

                if (document.Length != fileSize)
                {
                    document.Length = fileSize;
                    dbContextService.UpdateById(document);
                    ++updatedFilesCount;
                }
            }

            return updatedFilesCount;
        }

        public long WriteEveryThirdFileToFile(string accountId, TextWriter outputFile = null)
        {
            var outputPath = ".";
            var outputFileName = "writeEveryThirdFile.txt";
            long writedFilesCount = 0;

            Console.WriteLine("Reading Documents from Database...");

            outputFile ??= new StreamWriter(Path.Combine(outputPath, outputFileName));
            var documents = dbContextService.GetAll<Document>("[AccountId] = @accountId", accountId).ToList();

            for (int i = 2; i < documents.Count(); i += 3)
            {
                var filePath = documents[i].FilePath;
                var fileContent = File.ReadAllText(filePath);

                if (fileContent.Contains(TextToMatch))
                {
                    outputFile.WriteLine(fileContent);
                    ++writedFilesCount;
                }
                    
            }
            outputFile.Dispose();
            return writedFilesCount;
        }

        public void Dispose()
        {
            dbContextService.Close();            
        }
    }
}
