﻿namespace Api.Data;

public class Todo
{
    public int Id { get; set; }
    
    public required string Name { get; set; }

    public bool IsComplete { get; set; }
}