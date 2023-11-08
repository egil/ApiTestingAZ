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
            s.StatusCodeShouldBe(StatusCodes.Status201Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
            s.ContentShouldBeEquivalentTo(
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
            s.StatusCodeShouldBe(StatusCodes.Status201Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
            s.ContentShouldBeEquivalentTo(
                new TestTodo(
                    Id: 1,
                    Name: "Give presentation at Oredev",
                    IsComplete: false,
                    Created: TimeProvider.GetUtcNow(),
                    Modified: TimeProvider.GetUtcNow()));
        });
    }
}

