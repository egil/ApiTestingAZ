using TodoApi.Data;

namespace TodoApi.Todos;

public record class TodoDTO(
    int Id, 
    string Name, 
    bool IsCompleted, 
    DateTimeOffset Created, 
    DateTimeOffset Modified)
{
    internal TodoDTO(Todo todo) : this(todo.Id, todo.Name, todo.IsComplete, todo.Created, todo.Modified)
    { }
}
