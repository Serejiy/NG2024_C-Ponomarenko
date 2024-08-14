using System.Text.Json;

namespace ReportApp.Services;

public class SerializeReportModel
{
    public T SerializeModel<T>(string path)
    {
        var jsonContent = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(jsonContent);
    }
}
