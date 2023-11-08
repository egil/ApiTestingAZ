using TodoApi.Todos;

namespace TodoApi.Tests.Todos.CreateTests;

public partial class CreateTodoTests : TodoApiTestBase
{
    [Fact]
    public async Task Create_todo_using_api_DTOs()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Post
                .Json(new CreateTodoParameters(
                        Name: "Give presentation at Oredev",
                        IsComplete: false))
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(HttpStatusCode.Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
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
    public async Task Create_todo_using_test_DTOs()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act 
            s.Post
                .Json(new TestCreateParams(
                        Name: "Give presentation at Oredev",
                        IsComplete: false))
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(HttpStatusCode.Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
            s.ContentShouldBeJsonEquivalentTo(
                new TestTodo(
                    Id: 1,
                    Name: "Give presentation at Oredev",
                    IsComplete: false,
                    Created: TimeProvider.GetUtcNow(),
                    Modified: TimeProvider.GetUtcNow()));
        });
    }

    internal record class TestCreateParams(string Name, bool IsComplete);

    internal record class TestTodo(int Id, string Name, bool IsComplete, DateTimeOffset Created, DateTimeOffset Modified);
}

