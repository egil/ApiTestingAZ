namespace TodoApi.Tests.Todos;

internal record class TestTodo(
    int Id, 
    string Name, 
    bool IsComplete, 
    DateTimeOffset Created, 
    DateTimeOffset Modified);

