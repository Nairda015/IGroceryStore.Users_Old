using FluentValidation;
using IGroceryStore.Shared.Settings;

namespace IGroceryStore.Users.Settings;

internal class MongoDbSettings : SettingsBase<MongoDbSettings>, ISettings
{
    public static string SectionName => "Users:MongoDb";
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string BasketsCollectionName { get; set; }
    public string ProductsCollectionName { get; set; }
    public string UsersCollectionName { get; set; }
    public string ProjectionsCollectionName { get; set; }
    
    
    public MongoDbSettings()
    {
        RuleFor(x => ConnectionString).NotEmpty();
    }
}
