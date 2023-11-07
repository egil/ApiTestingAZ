namespace TodoApi;

[CollectionDefinition(nameof(ApiTestCollection))]
public sealed class ApiTestCollection : ICollectionFixture<TodoApiFixture>
{
}
