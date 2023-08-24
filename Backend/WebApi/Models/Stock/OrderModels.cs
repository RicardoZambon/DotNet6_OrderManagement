using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;

namespace Zambon.OrderManagement.WebApi.Models.Stock
{
    #region List

    public class OrdersListModel
    {
        public long ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CustomerID_Name { get; set; }
        public decimal Total { get; set; }
    }

    #endregion

    #region CRUD

    public class OrderInsertModel
    {
        public long CustomerID { get; set; }
    }

    public class OrderUpdateModel : OrderInsertModel
    {
        public long ID { get; set; }
    }

    public class OrderDisplayModel : OrderUpdateModel
    {
        public string? CustomerID_Name { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    public class OrderModelsProfiles : Profile
    {
        public OrderModelsProfiles()
        {
            CreateMap<Orders, OrdersListModel>()
                .ForMember(x => x.CustomerID_Name, x => x.MapFrom(c => c.Customer != null ? c.Customer.Name : null))
                .ForMember(x => x.Total, x => x.MapFrom(c => c.Products != null ? c.Products.Sum(p => p.Qty * p.UnitPrice) : 0));

            CreateMap<OrderInsertModel, Orders>();
            CreateMap<OrderUpdateModel, Orders>();
            CreateMap<Orders, OrderDisplayModel>()
                .ForMember(x => x.CustomerID_Name, x => x.MapFrom(c => c.Customer != null ? c.Customer.Name : null))
                .ForMember(x => x.Total, x => x.MapFrom(c => c.Products != null ? c.Products.Sum(p => p.Qty * p.UnitPrice) : 0));
        }
    }

    #endregion
}