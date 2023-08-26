using Zambon.OrderManagement.Core.Interfaces;

namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Model used to filter or sort entities lists.
    /// </summary>
    public class ListParametersModel : IListParameters
    {
        /// <summary>
        /// The number of rows to return.
        /// </summary>
        public int EndRow { get; set; }

        /// <summary>
        /// Dictionary of the entity properties to filter the returned list.
        /// </summary>
        public IDictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// The number of the first row to return.
        /// </summary>
        public int StartRow { get; set; }
    }
}