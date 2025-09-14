using UserApi.IntegrationTests.Setup;
using Xunit;

[CollectionDefinition("api")]
public class ApiCollection : ICollectionFixture<MySqlContainerFixture> {}