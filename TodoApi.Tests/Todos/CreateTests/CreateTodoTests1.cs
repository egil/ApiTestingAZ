namespace TodoApi.Tests.Todos.CreateTests;

public partial class CreateTodoTests : TodoApiTestBase
{
    public CreateTodoTests(TodoApiFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Create_todo_using_raw_json()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Post.Text("""
                {
                    "name": "Give presentation at Oredev",
                    "isComplete": false
                }
                """)
                .ContentType(MediaTypeNames.Application.Json)
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(HttpStatusCode.Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
            s.ContentShouldBeJsonEquivalentTo($$"""
                {
                    "id": 1,
                    "name": "Give presentation at Oredev",
                    "isComplete": false,
                    "created": "{{TimeProvider.GetUtcNow().ToString("O")}}",
                    "modified": "{{TimeProvider.GetUtcNow().ToString("O")}}"
                }
                """);
        });
    }
}
