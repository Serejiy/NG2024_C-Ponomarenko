using ClosedXML.Excel;
using ClosedXML.Report;
using ReportApp.Models;
using ReportApp.Models.Shop;

namespace ReportApp.Services.Shop;

public class ShopTemplateManagerService
{
    private static TemplateManagerService _templateService = new TemplateManagerService();

    private static Dictionary<string, Func<ShopReportModel, object>> KeyValuePairs { get; set; } = new Dictionary<string, Func<ShopReportModel, object>>
    {
        { "Name", r => r.PointOfPurchase },
        { "Seller", r => r.Seller },
        { "Items", r => r.Items }
    };

    private static Dictionary<string, Func<ShopReportModel, object>> GetKeyValuePairs()
    {
        return KeyValuePairs;
    }

    public static void FillingAndFormattingExcel(XLTemplate template, ShopReportConfiguration configuration, List<ShopReportModel> models, string type)
    {
        var worksheet = InitializeWorksheet(template, configuration);
        var groupAmount = 1;
        var lastDataColumn = configuration.LastColumn;

        int initialLastRow = configuration.LastRow;

        FillReportData(worksheet, configuration, models, groupAmount);

        FinalizeWorksheet(template, worksheet, configuration, type, lastDataColumn, initialLastRow);

        CentrationModel centrationData = new CentrationModel()
        {
            FirstDynamicColumn = configuration.FirstColumnNumber,
            LastDynamicColumn = configuration.LastColumnNumber,
            FirstDynamicRow = configuration.DefaultRow,
            LastDynamicRow = configuration.LastRow
        };

        _templateService.InCenter(worksheet, centrationData);
    }

    private static IXLWorksheet InitializeWorksheet(XLTemplate template, ShopReportConfiguration configuration)
    {
        var worksheet = template.Workbook.Worksheets.First();
        worksheet.SetShowGridLines(false);
        return worksheet;
    }

    private static void FillReportData(IXLWorksheet worksheet, ShopReportConfiguration configuration, List<ShopReportModel> models, int groupAmount)
    {
        var firstDataRow = configuration.DefaultRow;
        var firstDataColumn = configuration.FirstColumn;
        var lastDataColumn = configuration.LastColumn;


        foreach (var model in models)
        {
            for (int group = 1; group <= groupAmount; group++)
            {
                foreach (var item in model.Items)
                {
                    int column = firstDataColumn;
                    foreach (var property in GetKeyValuePairs())
                    {
                        if (property.Key.Equals("Items"))
                        {
                            var properties = new List<object>
                            {
                                item.Name,
                                item.Cost,
                                item.Quantity,
                                item.Notes
                            };

                            foreach (var value in properties)
                            {
                                if (value is not string)
                                {
                                    worksheet.Cell(firstDataRow, column).Value = Convert.ToDecimal(value);
                                }
                                else
                                {
                                    worksheet.Cell(firstDataRow, column).Value = value?.ToString();
                                }
                                column++;
                            }
                        }
                        else
                        {
                            worksheet.Cell(firstDataRow, column).Value = property.Value(model).ToString();
                            column++;
                        }
                    }
                    firstDataRow++;
                    configuration.LastRow++;
                }
            }
        }

        worksheet.Columns(configuration.FirstColumn, configuration.LastColumn).AdjustToContents();
    }

    private static void FinalizeWorksheet(XLTemplate template, IXLWorksheet worksheet, ShopReportConfiguration configuration, string type, int lastDataColumn, int initialLastRow)
    {
        var workingRange = worksheet.Range(configuration.ReportTitleRow, configuration.FirstColumn, configuration.LastRow, lastDataColumn);
        _templateService.FormatStyle(worksheet, configuration, configuration.LastRow);
        TemplateManagerService.GetDrawTemplate(worksheet, configuration, type, lastDataColumn, initialLastRow);
    }
}