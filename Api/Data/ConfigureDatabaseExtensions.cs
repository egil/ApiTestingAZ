using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public static class ConfigureDatabaseExtensions
{
    public static void AddTodoDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }
}
