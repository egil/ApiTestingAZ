using TodoApi.Todos;

namespace TodoApi.Tests.Todos.CreateTests;

[UsesVerify]
public partial class CreateTodoTests : TodoApiTestBase
{   
    [Fact]
    public async Task Create_todo_using_snapshots()
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

            s.StatusCodeShouldBe(StatusCodes.Status201Created);
        }).Verify();
    }
}
