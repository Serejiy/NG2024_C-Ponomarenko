using System.Text.Json;

namespace ReportApp.Services;

public class ConfigurationService
{
    public T LoadConfiguration<T>(string path)
    {
        var jsonContent = File.ReadAllText(path);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<T>(jsonContent, options);
    }
}