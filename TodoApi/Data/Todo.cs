using System.ComponentModel.DataAnnotations;

namespace TodoApi.Data;

public class Todo
{
    public int Id { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }

    public bool IsComplete { get; set; }

    public required DateTimeOffset Created { get; set; }

    public required DateTimeOffset Modified { get; set; }
}