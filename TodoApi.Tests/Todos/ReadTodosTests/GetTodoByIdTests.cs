using TodoApi.Tests.AblaExtensions;
using TodoApi.Todos;

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
        // Create a new todo in the database via the TodoDb context.
        using var db = Services.GetRequiredService<TodoDb>();
        var todo = new Todo { Name = "foo", Created = TimeProvider.GetUtcNow(), Modified = TimeProvider.GetUtcNow() };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        // Act
        var actualTodo = await Host.GetAsJson<TodoDto>($"/todos/{todo.Id}");

        // Assert
        actualTodo.Should().BeEquivalentTo(todo);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        // Create a new todo via API.
        var createdTodo = await Host
            .CreateJson(new CreateTodoParameters(Name: "Give presentation at Oredev", IsComplete: false), "/todos")
            .Receive<TodoDto>();

        // Act
        await Host.Scenario(s =>
        {
            s.Get.Url($"/todos/{createdTodo!.Id}");

            // Assert
            s.StatusCodeShouldBeOk();
            s.ContentShouldBeJsonEquivalentTo(createdTodo);
        });
    }
}
