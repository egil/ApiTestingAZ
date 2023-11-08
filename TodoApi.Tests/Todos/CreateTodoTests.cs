namespace TodoApi.Todos;

[UsesVerify]
public class CreateTodoTests : TodoApiTestBase
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

    [Fact]
    public async Task Create_todo_using_anonymous_objects()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Post
                .Json(new
                {
                    name = "Give presentation at Oredev",
                    isComplete = false
                })
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(HttpStatusCode.Created);
            s.ContentShouldBeJsonEquivalentTo(new
            {
                id = 1,
                name = "Give presentation at Oredev",
                isComplete = false,
                created = TimeProvider.GetUtcNow(),
                modified = TimeProvider.GetUtcNow()
            });
        });
    }

    [Fact]
    public async Task Create_todo_using_api_DTOs()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Post
                .Json(new AddOrUpdateTodoDto(
                        Name: "Give presentation at Oredev",
                        IsComplete: false))
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(HttpStatusCode.Created);
            s.ContentShouldBeJsonEquivalentTo(
                new TodoDto(
                    Id: 1,
                    Name: "Give presentation at Oredev",
                    IsComplete: false,
                    Created: TimeProvider.GetUtcNow(),
                    Modified: TimeProvider.GetUtcNow()));
        });
    }

    [Fact]
    public async Task Create_todo_using_snapshots()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Post
                .Json(new AddOrUpdateTodoDto(
                        Name: "Give presentation at Oredev",
                        IsComplete: false))
                .ToUrl("/todos");
            s.StatusCodeShouldBe(HttpStatusCode.Created);
        }).Verify();
    }
}
