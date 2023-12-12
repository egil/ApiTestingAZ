using Xunit.Abstractions;

namespace TodoApi.Tests.Todos.CreateTests;

public partial class CreateTodoTests : TodoApiTestBase
{
    public CreateTodoTests(TodoApiFixture fixture, ITestOutputHelper testOutputHelper) 
        : base(fixture, testOutputHelper)
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
            s.StatusCodeShouldBe(StatusCodes.Status201Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");

            // Compare the received JSON with the provided using semantic comparison of JSON
            s.ContentShouldBeJson($$"""
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
