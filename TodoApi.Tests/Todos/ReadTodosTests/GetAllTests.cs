using Xunit.Abstractions;

namespace TodoApi.Tests.Todos.ReadTodosTests;

public class GetAllTests : TodoApiTestBase
{
    public GetAllTests(TodoApiFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
    {
    }

    [Fact]
    public async Task GetAll_when_empty()
    {
        await Host.Scenario(s =>
        {
            s.Get.Url("/todos");

            s.ContentShouldBeJson("[]");
        });
    }

    [Fact]
    public async Task GetAll()
    {
        var expectedTodos = await CreateManyTodo(
            ("A", false),
            ("B", true));

        await Host.Scenario(s =>
        {
            s.Get.Url("/todos");

            s.ContentShouldBeEquivalentTo(expectedTodos);
        });
    }

    [Fact]
    public async Task GetAllCompleted_when_none()
    {
        var expectedTodos = await CreateManyTodo(
            ("A", false),
            ("B", false));

        await Host.Scenario(s =>
        {
            s.Get.Url("/todos/complete");

            s.ContentShouldBeJson("[]");
        });
    }

    [Fact]
    public async Task GetAllCompleted()
    {
        var completeTodoA = await CreateTodo("A", true);
        var newTodoB = await CreateTodo("B", false);
        var completeTodoC = await CreateTodo("C", true);

        await Host.Scenario(s =>
        {
            s.Get.Url("/todos/complete");

            s.ContentShouldBeEquivalentTo([completeTodoA, completeTodoC]);
        });
    }
}
