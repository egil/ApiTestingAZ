using TodoApi.Tests.Todos;

namespace TodoApi.Tests;

public abstract partial class TodoApiTestBase
{
    internal async Task<TestTodo> CreateTodo(
        string name = "Test todo",
        bool isComplete = false)
    {
        var result = await Host
            .CreateJson(new TestCreateParams(name, isComplete), "/todos")
            .Receive<TestTodo>();

        Assert.NotNull(result);

        return result;
    }

    internal async Task<IReadOnlyList<TestTodo>> CreateManyTodo(
        params (string Name, bool IsComplete)[] todos)
    {
        var result = new List<TestTodo>();

        // Explicitly awaiting each create in turn to ensure
        // the order todos are created in is the same as the
        // order passed into this method.
        // This ensures that assertions that assert on returned
        // array content works as expected, since order in arrays
        // is significant in JSON.
        foreach (var (name, isComplete) in todos)
        {
            result.Add(await CreateTodo(name, isComplete));
        }

        return result;
    }
}
