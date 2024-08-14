using ReportApp.Models;
using ReportApp.Models.Activity;
using ReportApp.Models.Shop;
using ReportApp.Services;


var request = new ConfigurationService();
var data = request.LoadConfiguration<RequestModel>("./Request/Request.json");

switch (data.Type)
{
    case "Activity":
        var activityService = new BaseReportGeneratorService();
        activityService.GenerateReport<ActivityReportConfiguration, ActivityReportModel>(data.PathToFile, data.Type);
        break;
    case "Shop":
        var shopService = new BaseReportGeneratorService();
        shopService.GenerateReport<ShopReportConfiguration, List<ShopReportModel>>(data.PathToFile, data.Type);
        break;
    default:
        Console.WriteLine("Error, this type of reports doesn't exists");
        break;
}
