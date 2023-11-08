using TodoApi.Data;

namespace TodoApi.Todos;

public record class TodoDto(
    int Id, 
    string Name, 
    bool IsComplete, 
    DateTimeOffset Created, 
    DateTimeOffset Modified)
{
    internal TodoDto(Todo todo) : this(todo.Id, todo.Name, todo.IsComplete, todo.Created, todo.Modified)
    { }
}
