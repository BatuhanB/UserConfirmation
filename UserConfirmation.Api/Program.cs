using Microsoft.EntityFrameworkCore;
using UserConfirmation.Data.Models;
using UserConfirmation.Services;
using UserConfirmation.Services.Confirmations;
using UserConfirmation.Shared.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<UserConfirmation.Data.DbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<UserConfirmation.Data.DbContext>();

builder.Services.AddMemoryCache();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

//builder.Services.AddHostedService<ConfirmationWorker>();

builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddScoped<IConfirmationService, ConfirmationService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("client",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});


var app = builder.Build();

app.UseCors("client");

app.UseAuthorization();

app.MapControllers();

app.Run();
