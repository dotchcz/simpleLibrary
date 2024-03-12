using Microsoft.Extensions.DependencyInjection;
using SimpleLibrary.BusinessLogic.Services;
using SimpleLibrary.Common.Abstraction;

namespace SimpleLibrary.BusinessLogic;

public static class Bootstrapper
{
    public static void AddBusinessLogic(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(Bootstrapper).Assembly);
        
        serviceCollection.AddScoped<IBooksService, BooksService>();
        serviceCollection.AddScoped<ILoanService, LoanService>();
        serviceCollection.AddScoped<IUserService, UserService>();
    }
}