using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;

namespace Zambon.OrderManagement.WebApi.Models.Stock
{
    #region List

    public class OrdersProductsListModel
    {
        public long ID { get; set; }
        public long ProductID { get; set; }
        public string? ProductID_Name { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    #endregion

    #region CRUD

    public class OrdersProductUpdateModel
    {
        public long ID { get; set; }
        public long ProductID { get; set; }
        public int Qty { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    public class OrdersProductModelsProfiles : Profile
    {
        public OrdersProductModelsProfiles()
        {
            CreateMap<OrdersProducts, OrdersProductsListModel>()
                .ForMember(x => x.ProductID_Name, x => x.MapFrom(op => op.Product != null ? op.Product.Name : null))
                .ForMember(x => x.Total, x => x.MapFrom(op => op.Qty * op.UnitPrice));

            CreateMap<OrdersProductUpdateModel, OrdersProducts>();
        }
    }

    #endregion
}