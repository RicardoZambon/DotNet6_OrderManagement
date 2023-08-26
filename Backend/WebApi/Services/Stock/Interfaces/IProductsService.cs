using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models.Stock;

namespace Zambon.OrderManagement.WebApi.Services.Stock.Interfaces
{
    /// <summary>
    /// Service for viewing and updating the <see cref="Products"/>.
    /// </summary>
    public interface IProductsService
    {
        /// <summary>
        /// Return a product by the ID.
        /// </summary>
        /// <param name="productId">The ID of the product to search for.</param>
        /// <returns>An instance of <see cref="ProductUpdateModel"/> representing the properties from the found <see cref="Products"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task<ProductUpdateModel> FindProductByIdAsync(long productId);
        /// <summary>
        /// Validate and insert a new product.
        /// </summary>
        /// <param name="productModel">The <see cref="ProductInsertModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="ProductUpdateModel"/> representing the properties from the inserted <see cref="Products"/> instance.</returns>
        /// <exception cref="EntityValidationFailureException">If the <see cref="ProductInsertModel"/> has validation errors.</exception>
        Task<ProductUpdateModel> InsertNewProductAsync(ProductInsertModel productModel);
        /// <summary>
        /// Return a list of products.
        /// </summary>
        /// <param name="parameters">The <see cref="IListParameters"/> instance for pagination and filtering the results.</param>
        /// <returns>A list of <see cref="ProductsListModel"/> representing the properties from the <see cref="Products"/> list.</returns>
        /// <exception cref="ArgumentNullException">If the <see cref="IListParameters" /> is null.</exception>
        IEnumerable<ProductsListModel> ListProducts(IListParameters parameters);
        /// <summary>
        /// Remove existing products.
        /// </summary>
        /// <param name="productIds">The list of product IDs to be deleted.</param>
        /// <returns>A task the represents the asynchronous delete operation.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task RemoveProductsAsync(long[] productIds);
        /// <summary>
        /// Validate and update an existing product.
        /// </summary>
        /// <param name="productModel">The <see cref="ProductUpdateModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="ProductUpdateModel"/> representing the properties from the updated <see cref="Products"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        /// <exception cref="EntityValidationFailureException">If the <see cref="ProductUpdateModel"/> has validation errors.</exception>
        Task<ProductUpdateModel> UpdateExistingProductAsync(ProductUpdateModel productModel);
    }
}