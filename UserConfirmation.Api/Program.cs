using Microsoft.EntityFrameworkCore;
using UserConfirmation.Data.Models;
using UserConfirmation.Services;
using UserConfirmation.Services.Confirmations;
using UserConfirmation.Services.Worker;
using UserConfirmation.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserConfirmation.Data.DbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<UserConfirmation.Data.DbContext>();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

//builder.Services.AddHostedService<ConfirmationWorker>();

builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddScoped<IConfirmationService, ConfirmationService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
