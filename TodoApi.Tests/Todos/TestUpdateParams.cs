namespace TodoApi.Tests.Todos;

internal record class TestUpdateParams(
    string Name, 
    bool IsComplete);