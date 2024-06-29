using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserConfirmation.Services.Accounts;
using UserConfirmation.Services.MessageQueue;

namespace UserConfirmation.Services;
public static class DependencyResolver
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {      
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<IMessageQueueService, MessageQueueService>();

        return services;
    }
}
