using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Todos;

public static class ReadTodoEndpoints
{
    internal static void Map(RouteGroupBuilder todoEndpoints)
    {
        todoEndpoints.MapGet("/", GetAllTodos).WithName("getAll");
        todoEndpoints.MapGet("/complete", GetCompleteTodos).WithName("getAllCompleted");
        todoEndpoints.MapGet("/{id}", GetTodo).WithName("getById");
    }

    public static async Task<Ok<TodoDto[]>> GetAllTodos(TodoDb db)
        => TypedResults.Ok(await db.Todos.Select(x => new TodoDto(x)).ToArrayAsync());

    public static async Task<Ok<TodoDto[]>> GetCompleteTodos(TodoDb db)
        => TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoDto(x)).ToArrayAsync());

    public static async Task<Results<Ok<TodoDto>, NotFound>> GetTodo(int id, TodoDb db)
        => await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(new TodoDto(todo))
                : TypedResults.NotFound();
}
