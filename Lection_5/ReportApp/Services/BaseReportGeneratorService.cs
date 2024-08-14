using ClosedXML.Report;
using ReportApp.Models;
using ReportApp.Models.Activity;
using ReportApp.Models.Shop;
using ReportApp.Services.Activity;
using ReportApp.Services.Shop;

namespace ReportApp.Services;

public class BaseReportGeneratorService
{
    private readonly ConfigurationService _reportConfigurationService = new ConfigurationService();
    private readonly TemplateManagerService _templateService = new TemplateManagerService();

    private readonly ActivityTemplateManagerService _activityReportDataService = new ActivityTemplateManagerService();
    private readonly ShopTemplateManagerService _shopReportDataService = new ShopTemplateManagerService();

    private readonly SerializeReportModel _serializService = new SerializeReportModel();

    private static Dictionary<string, Action<XLTemplate, ReportConfiguration, object, string>> GetGenerate = new Dictionary<string, Action<XLTemplate, ReportConfiguration, object, string>>
    {
        {
            "Shop",
            (activityTemplate, configuration, activityModel, type) =>
            {
                ShopTemplateManagerService.FillingAndFormattingExcel(activityTemplate, (ShopReportConfiguration)configuration, (List<ShopReportModel>)activityModel, type);
            }
        },
        {
            "Activity",
            (activityTemplate, configuration, activityModel, type) =>
            {
                ActivityTemplateManagerService.FillingAndFormattingExcel(activityTemplate, (ActivityReportConfiguration)configuration, (ActivityReportModel)activityModel, type);
                TemplateManagerService.FillHeader(activityTemplate, configuration, (ActivityReportModel)activityModel);
            }
        }
    };

    public void GenerateReport<RConf, RModel>(string pathToFile, string type)
    where RConf : ReportConfiguration
    where RModel : class
    {
        string pathToConfiguration = $"./ReportConfigurations/{type}.json";

        var configuration = _reportConfigurationService.LoadConfiguration<RConf>(pathToConfiguration);
        var activityTemplate = _templateService.GetReportTemplate(type);
        var activityModel = _serializService.SerializeModel<RModel>(pathToFile);

        if (GetGenerate.TryGetValue(type, out var generateTemplate))
        {
            generateTemplate(activityTemplate, configuration, activityModel, type);
        }
        else
        {
            throw new Exception($"Template for '{type}' not found");
        }

        activityTemplate.Generate();
        activityTemplate.SaveAs($"../../../Reports/{type}Report.xlsx");
    }
}
