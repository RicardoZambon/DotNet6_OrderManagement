using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Stock;

namespace Zambon.OrderManagement.WebApi.Services.Stock.Interfaces
{
    public interface IProductsService
    {
        Task<ProductUpdateModel> FindProductByIdAsync(long productId);
        Task<ProductUpdateModel> InsertNewProductAsync(ProductInsertModel productModel);
        IEnumerable<ProductsListModel> ListProducts(IListParameters parameters);
        Task RemoveProductsAsync(long[] productIds);
        Task<ProductUpdateModel> UpdateExistingProductAsync(ProductUpdateModel productModel);
    }
}