namespace Zambon.OrderManagement.Core.Interfaces
{
    public interface ISummaryParameters
    {
        IDictionary<string, object> Filters { get; set; }
    }
}