using Microsoft.Extensions.DependencyInjection;

namespace SimpleLibrary.Database;

public static class Bootstrapper
{
    public static void AddDatabase(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<LibraryDbContext>();
    }
    
}