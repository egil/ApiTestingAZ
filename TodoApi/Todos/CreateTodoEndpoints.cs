using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.Data;

namespace TodoApi.Todos;

public class CreateTodoEndpoints
{
    internal static void Map(RouteGroupBuilder todoEndpoints)
    {
        todoEndpoints.MapPost("/", CreateTodo).WithName("create");
    }

    public static async Task<Results<Created<TodoDto>, BadRequest, ValidationProblem>> CreateTodo(
        CreateTodoParameters todoItemDTO,
        TodoDb db,
        TimeProvider timeProvider)
    {
        var validationResult = new CreateTodoParameters.Validator().Validate(todoItemDTO);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToValidationProblemErrors());
        }

        var todoItem = new Todo
        {
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name,
            Created = timeProvider.GetUtcNow(),
            Modified = timeProvider.GetUtcNow(),
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        var result = new TodoDto(todoItem);

        return TypedResults.Created($"{TodoEndpoints.EndpointBaseUrl}/{result.Id}", result);
    }
}
