
using System.IO;
using System.Threading;

namespace SubscriptionDashboard.Services;

public class ConfigurationService
{
    private static Settings? _settings;
    private static ConfigurationService? _instance;
    public static ConfigurationService Instance => 
        LazyInitializer.EnsureInitialized(ref _instance, () => new ConfigurationService());

    public ConfigurationService()
    {
        var configurationText = File.ReadAllText("settings.json");
        _settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(configurationText);
    }
    
    public void UpdateSettings(Settings settings)
    {
        _settings = settings;
        var configurationText = System.Text.Json.JsonSerializer.Serialize(settings);
        File.WriteAllText("settings.json", configurationText);
    }
    public Settings Configuration => _settings;

}