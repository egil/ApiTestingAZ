using Microsoft.AspNetCore.Http.HttpResults;

namespace TodoApi.Todos;

public class UpdateTodoEndpoints
{
    internal static void Map(RouteGroupBuilder todoEndpoints)
    {
        todoEndpoints.MapPut("/{id}", UpdateTodo).WithName("update");
    }

    public static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> UpdateTodo(
        int id,
        UpdateTodoParameters parameters,
        TodoDb db,
        TimeProvider timeProvider)
    {
        var validationResult = new UpdateTodoParameters.Validator().Validate(parameters);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToValidationProblemErrors());
        }

        var todo = await db.Todos.FindAsync(id);

        if (todo is null)
            return TypedResults.NotFound();

        todo.Name = parameters.Name;
        todo.IsComplete = parameters.IsComplete;
        todo.Modified = timeProvider.GetUtcNow();

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
