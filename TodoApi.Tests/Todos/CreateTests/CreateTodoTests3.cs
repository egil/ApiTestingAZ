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
            // HERE BE SOME DRAGONS!
            s.Post
                .Json(new TodoApi.Todos.CreateTodoParameters(
                        Name: "Give presentation at KCDC",
                        IsComplete: false))
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(StatusCodes.Status201Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
            // HERE BE SOME DRAGONS!
            s.ContentShouldBeEquivalentTo(
                new TodoApi.Todos.TodoDto(
                    Id: 1,
                    Name: "Give presentation at KCDC",
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
                        Name: "Give presentation at KCDC",
                        IsComplete: false))
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(StatusCodes.Status201Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
            s.ContentShouldBeEquivalentTo(
                new TestTodo(
                    Id: 1,
                    Name: "Give presentation at KCDC",
                    IsComplete: false,
                    Created: TimeProvider.GetUtcNow(),
                    Modified: TimeProvider.GetUtcNow()));
        });
    }

    [Fact]
    public async Task Create_todo()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            // Create without sending default parameter isComplete
            s.Post
                .Json(new TestCreateParams(Name: "Give presentation at KCDC"))
                .ToUrl("/todos");

            // Assert
            s.StatusCodeShouldBe(StatusCodes.Status201Created);
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
            s.ContentShouldBeEquivalentTo(
                new TestTodo(
                    Id: 1,
                    Name: "Give presentation at KCDC",
                    IsComplete: false,
                    Created: TimeProvider.GetUtcNow(),
                    Modified: TimeProvider.GetUtcNow()));
        });
    }
}

