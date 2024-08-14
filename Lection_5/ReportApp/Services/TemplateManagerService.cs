using ClosedXML.Excel;
using ClosedXML.Report;
using ReportApp.Models;
using ReportApp.Models.Activity;
using ReportApp.Models.Entity;

namespace ReportApp.Services;

public class TemplateManagerService
{
    private readonly ConfigurationService _configurationService = new ConfigurationService();
    private static ActivityReportSettings Settings { get; set; }

    Dictionary<string, string> GetTemplate = new Dictionary<string, string>
    {
        {
            "Shop", "./Templates/ShopReport.xlsx"
        },
        {
            "Activity", "./Templates/ActivityReport.xlsx"
        }
    };

    public static Dictionary<string, Action<IXLWorksheet, ReportConfiguration, string, int, int>> GetDraw = new Dictionary<string, Action<IXLWorksheet, ReportConfiguration, string, int, int>>
    {
        {
            "Shop", (worksheet, configuration, type, actualLastColumn, initialLastRow) => DrawBorders(worksheet, configuration, type, actualLastColumn - 1, configuration.LastRow - 2)
        },
        {
            "Activity", (worksheet, configuration, type, actualLastColumn, initialLastRow) => DrawBorders(worksheet, configuration, type, actualLastColumn, configuration.LastRow)
        }
    };

    public XLTemplate GetReportTemplate(string type)
    {
        if (GetTemplate.TryGetValue(type, out var actionTemplate))
        {
            return new XLTemplate(actionTemplate);
        }
        else
        {
            throw new Exception($"Template for '{type}' not found");
        }
    }

    private static void FillSettings(ActivityReportModel model)
    {
        Admin? generatedByAdmin = null;
        Client? generatedByClient = null;

        if (model.GeneratedByAdmin != null)
        {
            generatedByAdmin = model.GeneratedByAdmin;
        }
        else
        {
            generatedByClient = model.GeneratedByClient;
        }

        Settings = new ActivityReportSettings
        {
            GeneratedByAdmin = generatedByAdmin,
            GeneratedByClient = generatedByClient,
            GeneratedFor = $"{model.ReportGeneratedFor.FirstName}  {model.ReportGeneratedFor.LastName}"
        };
    }

    public static void FillHeader(XLTemplate template, ReportConfiguration configuration, ActivityReportModel activityModel)
    {
        FillSettings(activityModel);

        if (Settings != null)
        {
            var isGeneratedByAdmin = Settings.GeneratedByAdmin != null ? true : false;
            template.AddVariable("GeneratedFor", Settings.GeneratedFor);

            if (isGeneratedByAdmin)
            {
                template.AddVariable("GeneratedBy",
                    $"{Settings.GeneratedByAdmin.FirstName} {Settings.GeneratedByAdmin.LastName} (Admin)");
            }
            else
            {
                template.AddVariable("GeneratedBy",
                    $"{Settings.GeneratedByClient.FirstName} {Settings.GeneratedByClient.LastName} (Client)");
            }
        }
    }

    public static void CleanTestData(XLTemplate template, ReportConfiguration configuration, int actualLastColumn)
    {
        var worksheet = template.Workbook.Worksheets.First();
        for (int row = configuration.DefaultRow; row <= configuration.LastRow; row++)
        {
            for (int column = actualLastColumn; column <= configuration.LastColumn; column++)
            {
                worksheet.Cell(row, column).Clear();
            }
        }
    }

    public static void GetDrawTemplate(IXLWorksheet worksheet, ReportConfiguration configuration, string type, int actualLastColumn, int initialLastRow)
    {
        if (GetDraw.TryGetValue(type, out var drawTemplate))
        {
            drawTemplate(worksheet, configuration, type, actualLastColumn, initialLastRow);
        }
        else
        {
            throw new Exception($"Template for '{type}' not found");
        }
    }

    public static void DrawBorders(IXLWorksheet worksheet, ReportConfiguration configuration, string type, int actualLastColumn, int initialLastRow)
    {
        for (int row = configuration.ReportTitleRow; row <= initialLastRow; row++)
        {
            for (int column = configuration.FirstColumn; column <= actualLastColumn; column++)
            {
                worksheet.Cell(row, column).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, column).Style.Border.OutsideBorderColor = XLColor.Black;
            }
        }
    }

    public void FormatStyle(IXLWorksheet worksheet, ShopReportConfiguration configuration, int finishedRow)
    {
        for (int row = configuration.DefaultRow; row < finishedRow - 1; row++)
        {
            if (row % 2 != 0)
            {
                for (int column = configuration.FirstColumn; column <= configuration.LastColumn - 1; column++)
                {
                    worksheet.Cell(row, column).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    worksheet.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
            }
            else
            {
                for (int column = configuration.FirstColumn; column <= configuration.LastColumn - 1; column++)
                {
                    worksheet.Cell(row, column).Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
            }
        }
    }

    public void InCenter(IXLWorksheet worksheet, CentrationModel data)
    {
        for (int row = data.FirstDynamicRow; row <= data.LastDynamicRow; row++)
        {
            for (int column = data.FirstDynamicColumn; column <= data.LastDynamicColumn; column++)
            {
                worksheet.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }
    }
}
