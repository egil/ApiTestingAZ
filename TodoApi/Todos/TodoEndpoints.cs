using Api.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Todos;

public static class TodoEndpoints
{
    private const string EndpointBaseUrl = "/todos";

    public static void MapTodosEndpoints(this WebApplication app)
    {
        var todoItems = app.MapGroup(EndpointBaseUrl).WithTags("todo");

        todoItems.MapGet("/complete", GetCompleteTodos).WithName("getAllCompleted");
        todoItems.MapGet("/", GetAllTodos).WithName("getAll");
        todoItems.MapGet("/{id}", GetTodo).WithName("getById");

        todoItems.MapPost("/", CreateTodo).WithName("create");
        todoItems.MapPut("/{id}", UpdateTodo).WithName("update");
        todoItems.MapDelete("/{id}", DeleteTodo).WithName("delete");
    }   

    private static async Task<Ok<TodoDTO[]>> GetAllTodos(TodoDb db)
        => TypedResults.Ok(await db.Todos.Select(x => new TodoDTO(x)).ToArrayAsync());

    private static async Task<Ok<TodoDTO[]>> GetCompleteTodos(TodoDb db)
        => TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoDTO(x)).ToArrayAsync());

    private static async Task<Results<Ok<TodoDTO>, NotFound>> GetTodo(int id, TodoDb db)
        => await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(new TodoDTO(todo))
                : TypedResults.NotFound();

    private static async Task<Results<Created<TodoDTO>, BadRequest, ValidationProblem>> CreateTodo(
        AddOrUpdateTodoDto todoItemDTO, 
        TodoDb db,
        TimeProvider timeProvider)
    {
        var validationResult = new AddOrUpdateTodoDto.Validator().Validate(todoItemDTO);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todoItem = new Todo
        {
            IsComplete = todoItemDTO.IsCompleted,
            Name = todoItemDTO.Name,
            Created = timeProvider.GetUtcNow(),
            Modified = timeProvider.GetUtcNow(),
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        var result = new TodoDTO(todoItem);

        return TypedResults.Created($"{EndpointBaseUrl}/{result.Id}", result);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> UpdateTodo(
        int id, 
        AddOrUpdateTodoDto todoItemDTO, 
        TodoDb db,
        TimeProvider timeProvider)
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
        todo.Modified = timeProvider.GetUtcNow();

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, TodoDb db)
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
