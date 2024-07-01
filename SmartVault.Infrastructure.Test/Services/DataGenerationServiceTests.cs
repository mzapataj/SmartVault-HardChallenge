using System.Data;
using Moq;
using SmartVault.Core.BusinessObjects;
using SmartVault.Core.Settings;
using SmartVault.Infrastructure.Services;
using SmartVault.Shared.Interfaces;
using Xunit;

namespace SmartVault.Infrastructure.Test.Services
{
    public class DataGenerationServiceTests
    {
        private readonly DataGenerationService dataGenerationService;
        private Mock<IDbContextService> mockDbContextService;
        private Mock<ISettings> mockSettings;

        public DataGenerationServiceTests()
        {
            mockDbContextService = new Mock<IDbContextService>();
            mockSettings = new Mock<ISettings>();
            mockDbContextService.SetupAllProperties();
            mockSettings.SetupAllProperties();
            mockSettings.Setup(x => x.AppName).Returns("SmartVault");
            mockSettings.Setup(x => x.DatabaseFileName).Returns("testdb.sqlite");
            dataGenerationService = new DataGenerationService(mockDbContextService.Object, mockSettings.Object);
        }

        [Fact]
        public void SeedDatabase()
        {
            long countDocument = 100 * 10_000;
            long countAccount = 100;
            long countUser = 100;
            mockDbContextService.Setup(x=>
                x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);
            mockDbContextService.Setup(x => x.CountAll<Document>(null, null)).Returns(countDocument);
            mockDbContextService.Setup(x => x.CountAll<Account>(null, null)).Returns(countAccount);
            mockDbContextService.Setup(x => x.CountAll<User>(null, null)).Returns(countUser);
            
            var result = dataGenerationService.SeedDatabase();
            
            mockDbContextService.Verify( x =>
                x.CreateDatabase(It.IsAny<string>()), Times.Once );
            mockDbContextService.Verify( x => 
                x.Insert(It.IsAny<Document>()), Times.Exactly((int)countDocument));
            mockDbContextService.Verify( x => 
                x.Insert(It.IsAny<Account>()), Times.Exactly((int)countAccount));
            mockDbContextService.Verify( x => 
                x.Insert(It.IsAny<User>()), Times.Exactly((int)countUser));
            Assert.Equal(result.documentData, countDocument.ToString());
            Assert.Equal(result.accountData, countAccount.ToString());
            Assert.Equal(result.userData, countUser.ToString());
        }
    }
}
