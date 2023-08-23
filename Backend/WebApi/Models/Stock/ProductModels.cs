using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;

namespace Zambon.OrderManagement.WebApi.Models.Stock
{
    #region List

    public class ProductsListModel
    {
        public long ID { get; set; }
        public string? Name { get; set; }
        public decimal UnitPrice { get; set; }
    }

    #endregion

    #region CRUD

    public class ProductInsertModel
    {
        public string? Name { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ProductUpdateModel : ProductInsertModel
    {
        public long ID { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    public class ProductModelsProfiles : Profile
    {
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