using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

public sealed class TodoDb : DbContext
{
    public DbSet<Todo> Todos => Set<Todo>();

    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

}