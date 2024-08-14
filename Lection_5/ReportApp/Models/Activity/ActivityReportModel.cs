using ReportApp.Models.Data;
using ReportApp.Models.Entity;

namespace ReportApp.Models.Activity;

public class ActivityReportModel
{
    public Client? GeneratedByClient { get; set; }

    public Admin? GeneratedByAdmin { get; set; }

    public DateTime WorkdayStartTime { get; set; }

    public DateTime WorkdayEndTime { get; set; }

    public string Office { get; set; }

    public Person ReportGeneratedFor { get; set; }

    public List<Complains>? Complains { get; set; }
}
