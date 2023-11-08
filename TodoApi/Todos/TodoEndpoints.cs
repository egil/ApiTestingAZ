namespace TodoApi.Todos;

public static class TodoEndpoints
{
    public const string EndpointBaseUrl = "/todos";

    public static void MapTodosEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup(EndpointBaseUrl)
            .WithTags("todo");

        ReadTodoEndpoints.Map(endpoints);
        CreateTodoEndpoints.Map(endpoints);
        UpdateTodoEndpoints.Map(endpoints);
        DeleteTodoEndpoints.Map(endpoints);
    }
}
