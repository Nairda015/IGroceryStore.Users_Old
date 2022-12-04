using System.Diagnostics;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Common;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Settings;
using IGroceryStore.Users.ReadModels;
using IGroceryStore.Users.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IGroceryStore.Users;

public class UsersModule : IModule
{
    public string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Users", "1.0.0.0");

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterHandlers<UsersModule>();
        
        RegisterMongoCollections(services, configuration);
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/health/{Name.ToLower()}", () => $"{Name} module is healthy")
            .WithTags(Constants.SwaggerTags.HealthChecks);

        endpoints.RegisterEndpoints<UsersModule>();
    }
    
    private static void RegisterMongoCollections(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetOptions<MongoDbSettings>();
        var mongoClient = new MongoClient(settings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
        
        services.AddScoped<IMongoCollection<UserReadModel>>(_ => mongoDatabase.GetCollection<UserReadModel>(settings.UsersCollectionName));
    }
}
