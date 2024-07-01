using Moq;
using SmartVault.Shared.Data;
using System;
using System.Data;
using SmartVault.Shared.Test.TestTables;
using Xunit;

namespace SmartVault.Shared.Test.Sql
{
    public class DbCommandServiceTests
    {
        private DbCommandService dbCommandService;
        private Mock<IDbCommand> commandMock;
        private Mock<IDbDataParameter> dbParameterMock;
        public DbCommandServiceTests()
        {   
            commandMock = new Mock<IDbCommand>();
            commandMock.SetupAllProperties();
            dbCommandService = new DbCommandService();
            dbCommandService.Init(commandMock.Object);
        }

        [Fact]
        public void CreateTable_Throws_NullReferenceException()
        {
            var exception = Assert.Throws<NotImplementedException>( () => dbCommandService.CreateTable<NotSupportedProperty>());
            Assert.NotNull(exception);
        }

        [Fact]
        public void CreateTable_WithPrimaryKey()
        {
            var expectedStr = @"CREATE TABLE [Bar] (
    [Id] INTEGER PRIMARY KEY, [Name] TEXT
)";

            dbCommandService.CreateTable<Bar>();

            Assert.Equal(expectedStr, commandMock.Object.CommandText);
        }

        [Fact]
        public void CreateTable_WithoutPrimaryKey()
        {
            var expectedStr = @"CREATE TABLE [NoPrimaryKey] (
    [Name] TEXT
)";

            dbCommandService.CreateTable<NoPrimaryKey>();

            Assert.Equal(expectedStr, commandMock.Object.CommandText);
        }


        [Fact]
        public void Insert()
        {
            var expectedStr = @"INSERT INTO [Bar] (Id, Name) VALUES (@IdBar, @NameBar)";
            dbParameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(x => x.CreateParameter()).Returns(dbParameterMock.Object);
            commandMock.Setup(x=> x.Parameters.Add(It.Is<IDbDataParameter>( y => y.ParameterName == "Id")));
            commandMock.Setup(x => x.Parameters.Add(It.Is<IDbDataParameter>(y => y.ParameterName == "Name")));

            var test = new Bar()
            {
                Id = 1,
                Name = "Test"
            };
            dbCommandService.Insert(test);
            
            commandMock.Verify(x => x.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(2));
            commandMock.Verify(x => x.ExecuteNonQuery(), Times.Once);
            Assert.Equal(expectedStr, commandMock.Object.CommandText);
        }

        [Fact]
        public void UpdateById()
        {
            var expectedStr = @"UPDATE [Bar] 
 SET [Name] = @NameBar) 
 WHERE [Id] = @IdBar";
            dbParameterMock = new Mock<IDbDataParameter>();
            commandMock.Setup(x => x.CreateParameter()).Returns(dbParameterMock.Object);
            commandMock.Setup(x => x.Parameters.Add(It.Is<IDbDataParameter>(y => y.ParameterName == "Id")));
            commandMock.Setup(x => x.Parameters.Add(It.Is<IDbDataParameter>(y => y.ParameterName == "Name")));

            var test = new Bar()
            {
                Id = 1,
                Name = "Updated Test"
            };
            dbCommandService.UpdateById(test);

            commandMock.Verify(x => x.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(2));
            commandMock.Verify(x => x.ExecuteNonQuery(), Times.Once);
            Assert.Equal(expectedStr, commandMock.Object.CommandText);
        }

    }
}
