using TodoApi.Tests;
using TodoApi.Tests.AblaExtensions;

namespace TodoApi.Tests.Todos.CreateTests;

public partial class CreateTodoTests : TodoApiTestBase
{
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
            s.Header(HeaderNames.Location).SingleValueShouldMatch(@"/todos/\d+$");
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
}
