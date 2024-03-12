using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleLibrary.Database;

namespace SimpleLibrary.BusinessLogic.Tests;

public class Fixture
{
    public readonly IServiceProvider ServiceProvider;

    public Fixture()
    {
        var root = CreateServiceProvider();
        var scope = root.CreateScope();
        ServiceProvider = scope.ServiceProvider;
    }
    
    private IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.AddBusinessLogic();
        
        return services.BuildServiceProvider();
    }
}