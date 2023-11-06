using Api.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Todos;

internal static class TodoEndpoints
{
    public static void MapTodosEndpoints(this WebApplication app)
    {
        var todoItems = app.MapGroup("/todoitems").WithTags("todo");

        todoItems.MapGet("/", GetAllTodos).WithName("getAll");
        todoItems.MapGet("/complete", GetCompleteTodos).WithName("getAllCompleted");
        todoItems.MapGet("/{id}", GetTodo).WithName("getById");
        todoItems.MapPost("/", CreateTodo).WithName("create");
        todoItems.MapPut("/{id}", UpdateTodo).WithName("update");
        todoItems.MapDelete("/{id}", DeleteTodo).WithName("delete");
    }

    private record class TodoItemDTO(int Id, string Name, bool IsCompleted)
    {
        public TodoItemDTO(Todo todo) : this(todo.Id, todo.Name, todo.IsComplete)
        { }
    }

    private record class AddOrUpdateTodoDto(string Name, bool IsCompleted)
    {
        public class Validator : AbstractValidator<AddOrUpdateTodoDto>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().WithName("name");
            }
        }
    }

    private static async Task<Ok<TodoItemDTO[]>> GetAllTodos(TodoDb db)
        => TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());

    private static async Task<Ok<TodoItemDTO[]>> GetCompleteTodos(TodoDb db)
        => TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToArrayAsync());

    private static async Task<Results<Ok<TodoItemDTO>, NotFound>> GetTodo([FromQuery] int id, TodoDb db)
        => await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(new TodoItemDTO(todo))
                : TypedResults.NotFound();

    private static async Task<Results<Created<TodoItemDTO>, BadRequest, ValidationProblem>> CreateTodo([FromBody] AddOrUpdateTodoDto todoItemDTO, TodoDb db)
    {
        var validationResult = new AddOrUpdateTodoDto.Validator().Validate(todoItemDTO);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todoItem = new Todo
        {
            IsComplete = todoItemDTO.IsCompleted,
            Name = todoItemDTO.Name
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        var result = new TodoItemDTO(todoItem);

        return TypedResults.Created($"/todoitems/{result.Id}", result);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> UpdateTodo([FromQuery] int id, [FromBody] AddOrUpdateTodoDto todoItemDTO, TodoDb db)
    {
        var validationResult = new AddOrUpdateTodoDto.Validator().Validate(todoItemDTO);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todo = await db.Todos.FindAsync(id);

        if (todo is null)
            return TypedResults.NotFound();

        todo.Name = todoItemDTO.Name;
        todo.IsComplete = todoItemDTO.IsCompleted;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTodo([FromQuery] int id, TodoDb db)
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
