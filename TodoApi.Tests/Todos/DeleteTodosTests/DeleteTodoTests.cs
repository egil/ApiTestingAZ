using Xunit.Abstractions;

namespace TodoApi.Tests.Todos.DeleteTodosTests;

public class DeleteTodoTests : TodoApiTestBase
{
    public DeleteTodoTests(TodoApiFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
    {
    }

    [Fact]
    public async Task Delete_with_invalid_id()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Delete.Url("/todos/42");

            // Assert
            s.ResponseShouldBeNotFound();
        });
    }

    [Fact]
    public async Task Delete_existing_todo()
    {
        // Arrange
        var todo = await CreateTodo();

        await Host.Scenario(s =>
        {
            // Act
            s.Delete.Url($"/todos/{todo.Id}");

            // Assert
            s.StatusCodeShouldBe(StatusCodes.Status204NoContent);
        });

        // Assert
        var result = await Host.TryGet<TestTodo>($"/todos/{todo.Id}");
        result.Should().BeNull();
    }
}
