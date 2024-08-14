using ReportApp.Models.Data;
using System.Text.Json.Serialization;

namespace ReportApp.Models.Shop;

public class ShopReportModel
{
    [JsonPropertyName("Point Of Purchase")]
    public string PointOfPurchase { get; set; }

    public List<Item> Items { get; set; }

    public string Seller { get; set; }
}
