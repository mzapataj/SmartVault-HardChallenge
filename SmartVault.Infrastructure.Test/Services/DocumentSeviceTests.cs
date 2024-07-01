using System;
using Moq;
using SmartVault.Core.BusinessObjects;
using SmartVault.Infrastructure.Services;
using SmartVault.Shared.Interfaces;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SmartVault.Infrastructure.Test.Services
{
    public class DocumentSeviceTests : IDisposable
    {
        private readonly DocumentService documentService;
        private Mock<IDbContextService> mockDbContextService;
        private const string directory = "Documents";
        private const string outputPath = "writeEveryThirdFile.txt";

        public DocumentSeviceTests()
        {
            mockDbContextService = new Mock<IDbContextService>();
            documentService = new DocumentService(mockDbContextService.Object);
        }

        [Fact]
        public void GetAllFileSizes()
        {
            var documents = new List<Document>();
            for (int i = 1; i <= 7; i++)
            {
                documents.Add(new ()
                {
                    Id = i,
                    Name = "Document"+i,
                    FilePath = Path.Combine(directory, $"Document{i}.txt"),
                    AccountId = i % 2,
                    Length = 2_000,
                });
            }
            documents[4].Length = 2_626;
            
            mockDbContextService.Setup( x => 
                x.GetAll<Document>(null, null)).Returns(documents);
            
            var filesUpdated = documentService.GetAllFileSizes();
            
            Assert.Equal(6, filesUpdated);
            mockDbContextService.Verify(x => 
                x.UpdateById(It.IsAny<Document>()), Times.Exactly(6));
        }
        
        [Fact]
        public void GetAllFileSizes_No_Updates()
        {
            var documents = new List<Document>();
            for (int i = 1; i <= 7; i++)
            {
                documents.Add(new ()
                {
                    Id = i,
                    Name = "Document"+i,
                    FilePath = Path.Combine(directory, $"Document{i}.txt"),
                    AccountId = i % 2,
                    Length = 2_626,
                });
            }
            documents[0].Length = 2_642;
            documents[2].Length = 2_638;
            documents[3].Length = 2_640;
            documents[5].Length = 2_640;
            mockDbContextService.Setup( x => 
                x.GetAll<Document>(null, null)).Returns(documents);
            
            var filesUpdated =documentService.GetAllFileSizes();
            
            Assert.Equal(0, filesUpdated);
            mockDbContextService.Verify(x => x.UpdateById(It.IsAny<Document>()), Times.Never);
        }
        
        [Fact]
        public void WriteEveryThirdFileToFile_Zero_Contains_Match_Text()
        {
            var expectedFileContent = string.Empty;
            string accountId = "1";
            var documents = new List<Document>();
            for (int i = 1; i <= 7; i++)
            {
                documents.Add(new ()
                {
                    Id = (i+5) % 7 + 1,
                    Name = "Document"+(i+5) % 7 + 1,
                    FilePath = Path.Combine(directory, $"Document{(i+5) % 7 + 1}.txt"),
                    AccountId = int.Parse(accountId)
                });
            }
            mockDbContextService.Setup( x => 
                x.GetAll<Document>(It.Is<string>( y => y == "[AccountId] = @accountId"),
            accountId, null)).Returns(documents);

            documentService.WriteEveryThirdFileToFile(accountId);

            
            Assert.Equal(expectedFileContent, File.ReadAllText(outputPath));
        }

        [Fact]
        public void WriteEveryThirdFileToFile_Two_Contains_Match_Text()
        {
            var expectedFileContent = $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test documentSmith Property{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}Smith PropertyThis is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}{Environment.NewLine}";
            string accountId = "1";
            string directory = "Documents";
            var documents = new List<Document>();
            for (int i = 1; i <= 7; i++)
            {
                documents.Add(new ()
                {
                    Id = i,
                    Name = "Document"+i,
                    FilePath = Path.Combine(directory, $"Document{i}.txt"),
                    AccountId = int.Parse(accountId)
                });
            }
            mockDbContextService.Setup( x => 
                x.GetAll<Document>(It.Is<string>( y => y == "[AccountId] = @accountId"),
            accountId, null)).Returns(documents);

            documentService.WriteEveryThirdFileToFile(accountId);

            
            Assert.Equal(expectedFileContent, File.ReadAllText(outputPath));
        }

        public void Dispose()
        {
            File.Delete(outputPath);
        }
    }
}
