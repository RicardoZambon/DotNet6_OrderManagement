using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;

namespace Zambon.OrderManagement.WebApi.Models.Stock
{
    #region List

    /// <summary>
    /// Model representation when returning a list of <see cref="Products"/>.
    /// </summary>
    public class ProductsListModel
    {
        /// <summary>
        /// The ID from the <see cref="Products"/> instance.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The name from the <see cref="Products"/> instance.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The unit price from the <see cref="Products"/> instance.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }

    #endregion

    #region CRUD

    /// <summary>
    /// Model representation when inserting a new entity into <see cref="Products"/>.
    /// </summary>
    public class ProductInsertModel
    {
        /// <summary>
        /// The name from the <see cref="Products"/> instance.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The unit price from the <see cref="Products"/> instance.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// Model representation when returning or updating a entity into <see cref="Products"/>.
    /// </summary>
    public class ProductUpdateModel : ProductInsertModel
    {
        /// <summary>
        /// The ID from the <see cref="Products"/> instance.
        /// </summary>
        public long ID { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    /// <summary>
    /// Configure profiles for mapping <see cref="Products"/> class with the model classes.
    /// </summary>
    public class ProductModelsProfiles : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModelsProfiles"/> class.
        /// </summary>
        public ProductModelsProfiles()
        {
            CreateMap<Products, ProductsListModel>();

            CreateMap<ProductInsertModel, Products>();
            CreateMap<ProductUpdateModel, Products>()
                .ReverseMap();
        }
    }

    #endregion
}