using Xunit.Abstractions;

namespace TodoApi.Tests.Todos.UpdateTodosTests;

public class UpdateTodoTests : TodoApiTestBase
{
    public UpdateTodoTests(TodoApiFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
    {
    }

    [Fact]
    public async Task Update_with_invalid_id()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Put
                .Json(new TestUpdateParams(
                    "Todo doesnt exists",
                    true))
                .ToUrl("/todos/42");

            // Assert
            s.ResponseShouldBeNotFound();
        });
    }

    [Fact]
    public async Task Update_with_invalid_json()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Put
                .Text("""
                    "todo" = "invalid json"
                """)
                .ContentType(MediaTypeNames.Application.Json)
                .ToUrl("/todos/42");

            // Assert
            s.ResponseShouldBeBadRequest(detail: "The request body contains invalid or incorrectly formatted JSON");
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Update_with_missing_content(string? newName)
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Put
                .Json(new
                {
                    name = newName,
                    isComplete = true
                })
                .ToUrl($"/todos/42");

            // Assert
            s.ResponseShouldBeBadRequest(
                title: "One or more validation errors occurred.",
                errors: [("name", ["'name' must not be empty."])]);
        });
    }

    [Fact]
    public async Task Update_todo()
    {
        // Arrange
        var todo = await CreateTodo(name: "Demo update tests", isComplete: false);
        TimeProvider.Advance(TimeSpan.FromMinutes(1));

        // Act
        await Host.Scenario(s =>
        {
            s.Put
                .Json(new TestUpdateParams(todo.Name, true))
                .ToUrl($"/todos/{todo.Id}");

            // Assert
            s.StatusCodeShouldBe(StatusCodes.Status204NoContent);
            s.ContentShouldBe("");
        });

        // Assert
        var result = await Host.GetAsJson<TestTodo>($"/todos/{todo.Id}");
        result.Should().BeEquivalentTo(todo with
        {
            IsComplete = true,
            Modified = TimeProvider.GetUtcNow(),
        });
    }
}
