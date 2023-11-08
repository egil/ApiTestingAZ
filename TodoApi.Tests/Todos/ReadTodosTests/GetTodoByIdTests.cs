namespace TodoApi.Tests.Todos.ReadTodosTests;

public class GetTodoByIdTests : TodoApiTestBase
{
    public GetTodoByIdTests(TodoApiFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetById_using_servers_DTOs()
    {
        // Arrange
        // This gets the DbContext from the API and uses that 
        // to set up preconditions for the test.
        using var db = Services.GetRequiredService<TodoDb>();

        var todo = new Todo
        {
            Name = "Demo creating new todo using DbContext directly",
            IsComplete = true,
            Created = TimeProvider.GetUtcNow(),
            Modified = TimeProvider.GetUtcNow()
        };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        // Act
        var actualTodo = await Host.GetAsJson<TestTodo>($"/todos/{todo.Id}");

        // Assert
        actualTodo.Should().BeEquivalentTo(todo);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        // Create a new todo via API.
        var createdTodo = await CreateTodo(
            name: "Demo creating new todo item using API",
            isComplete: false);

        // Act
        await Host.Scenario(s =>
        {
            s.Get.Url($"/todos/{createdTodo!.Id}");

            // Assert
            s.StatusCodeShouldBeOk();
            s.ContentShouldBeEquivalentTo(createdTodo);
        });
    }

    [Fact]
    public async Task GetById_invalid_id()
    {
        // Arrange
        await Host.Scenario(s =>
        {
            // Act
            s.Get.Url($"/todos/{42}");

            // Assert
            s.StatusCodeShouldBe(StatusCodes.Status404NotFound);
            s.ContentShouldBeProblemDetails(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Status = 404,
                Title = "Not Found"
            });
        });
    }
}
