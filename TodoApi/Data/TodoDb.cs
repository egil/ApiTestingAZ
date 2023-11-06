using Api.Data;
using Microsoft.EntityFrameworkCore;

class TodoDb : DbContext
{
    public DbSet<Todo> Todos => Set<Todo>();

    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

}