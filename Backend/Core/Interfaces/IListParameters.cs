namespace Zambon.OrderManagement.Core.Interfaces
{
    public interface IListParameters : ISummaryParameters
    {
        int EndRow { get; set; }
        int StartRow { get; set; }
    }
}