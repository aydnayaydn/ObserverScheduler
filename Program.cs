using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ObserverScheduler.Abstractions;
using ObserverScheduler.Api;
using ObserverScheduler.Api.Middlewares;
using ObserverScheduler.Data;
using ObserverScheduler.Extensions;
using ObserverScheduler.Repositories;
using ObserverScheduler.Service;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MainContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
Console.WriteLine("Program Started");

builder.Services.AddMongoClientSingleton();
Console.WriteLine("Mongo Client Added");
builder.Services.AddAppBusinessServices();
Console.WriteLine("Business Services Added");
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddJobBackgroundService();
Console.WriteLine("Job Background Service Added");


BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); 
Console.WriteLine("Bakcground Service Added");

var app = builder.Build();

// TenantMiddleware'i ekle
app.UseMiddleware<TenantMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    // Request scope içinde IUserService çözümlemesi
    var userService = context.RequestServices.GetRequiredService<IUserService>();
    var jobService = context.RequestServices.GetRequiredService<IJobService>();
    var customAuthMiddleware = new CustomAuthorizationMiddleware(next, userService, jobService);
    await customAuthMiddleware.InvokeAsync(context);
});

app.ConfigureEndpoints();
app.ConfigureUserEndpoints();
app.ConfigureJobEndpoints();

app.Run();