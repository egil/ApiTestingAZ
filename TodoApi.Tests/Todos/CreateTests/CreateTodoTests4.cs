using TodoApi.Todos;

namespace TodoApi.Tests.Todos.CreateTests;

public partial class CreateTodoTests : TodoApiTestBase
{   
    [Fact]
    public async Task Create_todo_using_snapshots()
    {
        // Arrange
        var result = await Host.Scenario(s =>
        {
            // Act
            s.Post
                .Json(new TodoApi.Tests.Todos.TestCreateParams(
                        Name: "Give presentation at KCDC",
                        IsComplete: false))
                .ToUrl("/todos");

            s.StatusCodeShouldBe(StatusCodes.Status201Created);
        });

        // Assert - using semantic comparison of the entire
        // IScenarioResult from Alba
        await Verify(result);
    }
}
