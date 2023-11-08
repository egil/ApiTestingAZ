using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.Data;

namespace TodoApi.Todos;

public static class DeleteTodoEndpoints
{
    internal static void Map(RouteGroupBuilder todoEndpoints)
    {
        todoEndpoints.MapDelete("/{id}", DeleteTodo).WithName("delete");
    }

    public static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, TodoDb db)
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}
