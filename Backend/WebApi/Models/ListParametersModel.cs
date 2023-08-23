using Zambon.OrderManagement.Core.Interfaces;

namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Generic parameter to filter or sort entities list.
    /// </summary>
    public class ListParametersModel : IListParameters
    {
        /// <summary>
        /// End row to return.
        /// </summary>
        public int EndRow { get; set; }

        /// <summary>
        /// List of entity properties to filter with the value.
        /// </summary>
        public IDictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Start row to return, default 0.
        /// </summary>
        public int StartRow { get; set; }
    }
}