using ClosedXML.Excel;
using ClosedXML.Report;
using ReportApp.Models;
using ReportApp.Models.Activity;
using ReportApp.Models.Entity;

namespace ReportApp.Services.Activity;

public class ActivityTemplateManagerService
{
    private readonly TemplateManagerService _templateService = new TemplateManagerService();

    private static Dictionary<string, Func<ActivityReportModel, object>> KeyValuePairs { get; set; } = new Dictionary<string, Func<ActivityReportModel, object>>
    {
        { "FirstName", r => r.ReportGeneratedFor.FirstName },
        { "LastName", r => r.ReportGeneratedFor.LastName },
        { "Day", r => r.WorkdayStartTime.ToShortDateString() },
        { "Office", r => r.Office },
        { "Additional Info", r => r.Complains }
    };

    public static void FillingAndFormattingExcel(XLTemplate template, ActivityReportConfiguration configuration, ActivityReportModel model, string type)
    {
        var worksheet = InitializeWorksheet(template, configuration);
        var (groupAmount, lastDataColumn) = AdjustForAdminGeneratedReport(template, configuration, model);

        InitializeKeyValuePairs(model.GeneratedByAdmin);

        int initialLastRow = configuration.LastRow;

        FillReportData(worksheet, configuration, model, groupAmount);

        FinalizeWorksheet(worksheet, configuration, type, lastDataColumn, initialLastRow);
    }

    private static IXLWorksheet InitializeWorksheet(XLTemplate template, ActivityReportConfiguration configuration)
    {
        var worksheet = template.Workbook.Worksheets.First();
        worksheet.SetShowGridLines(false);
        return worksheet;
    }

    private static (int, int) AdjustForAdminGeneratedReport(XLTemplate template, ActivityReportConfiguration configuration, ActivityReportModel model)
    {
        int groupAmount = 2;
        int lastDataColumn = configuration.LastColumn;

        if (model.GeneratedByAdmin == null)
        {
            groupAmount = 1;
            lastDataColumn -= 3;
            DeleteUnusedColumns(template, configuration, lastDataColumn);
            AdjustReportTitle(template, configuration, lastDataColumn);
            TemplateManagerService.CleanTestData(template, configuration, lastDataColumn);
        }

        return (groupAmount, lastDataColumn);
    }

    private static void DeleteUnusedColumns(XLTemplate template, ActivityReportConfiguration configuration, int lastDataColumn)
    {
        var worksheet = template.Workbook.Worksheets.First();
        worksheet.Range(configuration.FirstRowForDynamicGroup, lastDataColumn + 1, configuration.FirstRowForDynamicGroup, configuration.LastColumn)
            .Delete(XLShiftDeletedCells.ShiftCellsLeft);
        worksheet.Range(configuration.FirstRowForStaticGroup, lastDataColumn + 1, configuration.FirstRowForStaticGroup, configuration.LastColumn)
            .Delete(XLShiftDeletedCells.ShiftCellsLeft);
    }

    private static void AdjustReportTitle(XLTemplate template, ActivityReportConfiguration configuration, int lastDataColumn)
    {
        var worksheet = template.Workbook.Worksheets.First();
        var firstDataColumn = configuration.FirstColumn;
        var titleCell = worksheet.Cell(configuration.ReportTitleRow, firstDataColumn);
        var style = titleCell.Style;
        var title = titleCell.Value.ToString();

        var previousRange = worksheet.Range(configuration.ReportTitleRow, firstDataColumn, configuration.ReportTitleRow, configuration.LastColumn).Unmerge();
        previousRange.Clear();

        var newRange = worksheet.Range(configuration.ReportTitleRow, firstDataColumn, configuration.ReportTitleRow, lastDataColumn).Merge();
        newRange.Style = style;
        newRange.Value = title;
    }

    private static void InitializeKeyValuePairs(Admin admin)
    {
        if (admin != null)
        {
            KeyValuePairs.Add("Name", r => r.GeneratedByAdmin.PreferedName);
            KeyValuePairs.Add("Pronouns", r => r.GeneratedByAdmin.Pronouns);
            KeyValuePairs.Add("Works At", r => r.GeneratedByAdmin.City);
        }
    }

    private static void FillReportData(IXLWorksheet worksheet, ActivityReportConfiguration configuration, ActivityReportModel model, int groupAmount)
    {
        int firstDataRow = configuration.DefaultRow;
        int firstDataColumn = configuration.FirstColumn;

        for (int group = 1; group <= groupAmount; group++)
        {
            for (int row = 0; row < model.Complains.Count; row++)
            {
                int column = firstDataColumn;
                foreach (var property in KeyValuePairs)
                {
                    SetCellValue(worksheet, firstDataRow, row, column, property, model);
                    column++;
                }
                configuration.LastRow = Math.Max(configuration.LastRow, firstDataRow + row);
            }
        }
    }

    private static void SetCellValue(IXLWorksheet worksheet, int firstDataRow, int row, int column, KeyValuePair<string, Func<ActivityReportModel, object>> property, ActivityReportModel model)
    {
        if (property.Key.Equals("Additional Info"))
        {
            worksheet.Cell(firstDataRow + row, column).Value = model.Complains[row].Description;
        }
        else
        {
            worksheet.Cell(firstDataRow + row, column).Value = property.Value(model).ToString();
        }

        SetCellStyle(worksheet, firstDataRow + row, column);
    }

    private static void SetCellStyle(IXLWorksheet worksheet, int row, int column)
    {
        if (row % 2 == 0)
        {
            worksheet.Cell(row, column).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
            worksheet.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }
        else
        {
            worksheet.Cell(row, column).Style.Fill.BackgroundColor = XLColor.White;
            worksheet.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        }
    }

    private static void FinalizeWorksheet(IXLWorksheet worksheet, ActivityReportConfiguration configuration, string type, int lastDataColumn, int initialLastRow)
    {
        var workingRange = worksheet.Range(configuration.ReportTitleRow, configuration.FirstColumn, configuration.LastRow, lastDataColumn);
        worksheet.Columns(configuration.FirstColumn, configuration.LastColumn).AdjustToContents();
        TemplateManagerService.GetDrawTemplate(worksheet, configuration, type, lastDataColumn, initialLastRow);
    }
}
