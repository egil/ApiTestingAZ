using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public static class ConfigureDatabaseExtensions
{
    public static void AddTodoDatabase(this WebApplicationBuilder builder)
    {
        // Uncomment to use in memory database. Comment out other builder.Services.AddDbContext further down
        //builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
        builder.Services.AddDbContext<TodoDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }
}
