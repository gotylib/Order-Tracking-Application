using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderTracking.API.Middleware;
using OrderTracking.API.Services;
using OrderTracking.Data;
using OrderTracking.Data.Repositories;
using OrderTracking.Service.BackgroundServices;
using OrderTracking.Service.Interfaces;
using OrderTracking.Service.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Order Tracking API",
        Version = "v1",
        Description = "API для отслеживания статуса заказов"
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderTrackingDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSignalR();

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
        Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672"),
        UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "guest"
    };
    return factory;
});

builder.Services.AddSingleton<IMessagePublisher, RabbitMQMessagePublisher>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IHubContextWrapper, SignalRHubContextWrapper>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddHostedService<OrderStatusChangeConsumer>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("OrderTracking.API")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("OrderTracking.API"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderTrackingDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseRouting();

// Добавление Exception Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderTracking.API.Hubs.OrderTrackingHub>("/orderTrackingHub");

app.Run();
