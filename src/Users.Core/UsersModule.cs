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

public static class UsersModule
{
    public static string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Users", "1.0.0.0");
    public static void RegisterMongoCollections(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetOptions<MongoDbSettings>();
        var mongoClient = new MongoClient(settings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
        
        services.AddScoped<IMongoCollection<UserReadModel>>(_ => mongoDatabase.GetCollection<UserReadModel>(settings.UsersCollectionName));
    }
}
