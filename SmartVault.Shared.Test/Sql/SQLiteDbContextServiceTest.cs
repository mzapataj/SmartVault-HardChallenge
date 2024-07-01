using System;
using System.Data;
using System.IO;
using JetBrains.Annotations;
using Moq;
using SmartVault.Shared.Interfaces;
using SmartVault.Shared.Sql;
using SmartVault.Shared.Test.TestTables;
using Xunit;

namespace SmartVault.Shared.Test.Sql
{
    [TestSubject(typeof(SQLiteDbContextService))]
    public class SQLiteDbContextServiceTest
    {
        private Mock<IDbConnection> mockConnection;
        private Mock<IDbCommandService> mockCommandService;
        private Mock<IDbCommand> mockCommand;
        private SQLiteDbContextService dbContextService;
        private string databaseFilename;

        public SQLiteDbContextServiceTest()
        {
            mockConnection = new Mock<IDbConnection>();
            mockCommand = new Mock<IDbCommand>();
            mockCommandService = new Mock<IDbCommandService>();

            mockConnection.SetupAllProperties();
            mockCommandService.SetupAllProperties();

            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);
            mockCommand.Setup(x => x.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
            
            databaseFilename = "test" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var connectionStrings = string.Format($"data source={databaseFilename}");
            dbContextService = new SQLiteDbContextService(mockConnection.Object, mockCommandService.Object);
        }

        [Fact]
        public void CreateDatabase()
        {
            dbContextService.CreateDatabase(databaseFilename);

            var isDatabaseCreated = File.Exists(databaseFilename);
            
            Assert.True(isDatabaseCreated);
            File.Delete(databaseFilename);
        }
        
        [Fact]
        public void CreateTable()
        {
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            dbContextService.CreateDatabase(databaseFilename);
            dbContextService.Open();
            dbContextService.CreateTable<Bar>();
            
            mockCommandService.Verify(x => x.CreateTable<Bar>(), Times.Once);
        }
        
        [Fact]
        public void Insert()
        {
            var bar = new Bar()
            {
                Id = 1,
                Name = "Name"
            };
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            
            dbContextService.CreateDatabase(databaseFilename);
            dbContextService.Open();
            dbContextService.Insert(bar);
            
            mockCommandService.Verify(x => x.Insert(It.Is<Bar>( x => x.Id == bar.Id)), Times.Once);
        }
        
        [Fact]
        public void UpdateById()
        {
            var bar = new Bar()
            {
                Id = 1,
                Name = "Update Name"
            };
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            
            dbContextService.CreateDatabase(databaseFilename);
            dbContextService.Open();
            dbContextService.UpdateById(bar);
            
            mockCommandService.Verify(x => x.UpdateById(It.Is<Bar>( x => x.Id == bar.Id)), Times.Once);
        }
        
        [Fact]
        public void CountAll()
        {
            var expectedQuery = "SELECT COUNT(*) FROM [Bar];";
            long expectedCount = 10;
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(expectedCount);
            
            dbContextService.CreateDatabase(databaseFilename);
            dbContextService.Open();
            var resultCount = dbContextService.CountAll<Bar>();
            
            mockCommand.Verify(x => x.ExecuteScalar(), Times.Once);
            Assert.Equal(expectedCount, resultCount);
        }
        
        [Fact]
        public void GetAll()
        {
            var expectedQuery = "SELECT COUNT(*) FROM [Bar];";
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>()))
                .Returns( new Mock<IDataReader>().Object);
            
            dbContextService.CreateDatabase(databaseFilename);
            dbContextService.Open();
            dbContextService.GetAll<Bar>();
            
            mockCommand.Verify(x => x.ExecuteReader(It.IsAny<CommandBehavior>()), Times.Once);
        }
        [Fact]
        public void Get()
        {
            var expectedQuery = "SELECT COUNT(*) FROM [Bar];";
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>()))
                .Returns( new Mock<IDataReader>().Object);
            
            dbContextService.CreateDatabase(databaseFilename);
            dbContextService.Open();
            dbContextService.GetAll<Bar>("Id = $Id", "10");
            
            mockCommand.Verify(x => x.ExecuteReader(It.IsAny<CommandBehavior>()), Times.Once);
        }
    }
}