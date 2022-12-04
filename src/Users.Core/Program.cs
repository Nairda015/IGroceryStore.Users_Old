using System.Reflection;
using FluentValidation;
using IGroceryStore.API;
using IGroceryStore.API.Configuration;
using IGroceryStore.API.Middlewares;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Users;
using IGroceryStore.Users.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.ConfigureSystemManager();
builder.ConfigureLogging();
builder.ConfigureAuthentication();
builder.ConfigureMassTransit();
builder.ConfigureSwagger();
builder.ConfigureOpenTelemetry();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ISnowflakeService, SnowflakeService>();
builder.Services.RegisterHandlers<IApiMarker>();
builder.Services.RegisterMongoCollections(builder.Configuration);

builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddValidatorsFromAssemblies(
    new List<Assembly> { typeof(IApiMarker).Assembly },
    includeInternalTypes: true);

//**********************************//
var app = builder.Build();
//**********************************//

app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet($"/api/health/{UsersModule.Name.ToLower()}", () => $"{UsersModule.Name} module is healthy")
    .WithTags(Constants.SwaggerTags.HealthChecks);
app.RegisterEndpoints<IApiMarker>();

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGroceryStore"); });

app.MapFallbackToFile("index.html");
app.Run();