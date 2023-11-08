namespace TodoApi.Tests;

[CollectionDefinition(nameof(ApiTestCollection))]
public sealed class ApiTestCollection : ICollectionFixture<TodoApiFixture>
{
}
