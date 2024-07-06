using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserConfirmation.Services.Accounts;
using UserConfirmation.Services.CacheStore;
using UserConfirmation.Services.MessageQueue;

namespace UserConfirmation.Services;
public static class DependencyResolver
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {      
        services.AddScoped<ITempPasswordStore, TempPasswordStore>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IMessageQueueService, MessageQueueService>();

        return services;
    }
}
