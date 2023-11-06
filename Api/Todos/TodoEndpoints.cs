using Api.Data;
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

    private class TodoItemDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public bool IsComplete { get; set; }

        public TodoItemDTO() { }

        public TodoItemDTO(Todo todoItem)
            => (Id, Name, IsComplete) = (todoItem.Id, todoItem.Name, todoItem.IsComplete);
    }

    private class AddedOrChangedTodoItemDTO
    {
        public string? Name { get; set; }

        public bool IsComplete { get; set; }
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

    private static async Task<Results<Created<TodoItemDTO>, BadRequest>> CreateTodo([FromBody] AddedOrChangedTodoItemDTO todoItemDTO, TodoDb db)
    {
        var todoItem = new Todo
        {
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        var result = new TodoItemDTO(todoItem);

        return TypedResults.Created($"/todoitems/{result.Id}", result);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> UpdateTodo([FromQuery] int id, [FromBody] AddedOrChangedTodoItemDTO todoItemDTO, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) 
            return TypedResults.NotFound();

        todo.Name = todoItemDTO.Name;
        todo.IsComplete = todoItemDTO.IsComplete;

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
