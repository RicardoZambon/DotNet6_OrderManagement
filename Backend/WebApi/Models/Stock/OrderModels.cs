using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;

namespace Zambon.OrderManagement.WebApi.Models.Stock
{
    #region List

    /// <summary>
    /// Model representation when returning a list of <see cref="Orders"/>.
    /// </summary>
    public class OrdersListModel
    {
        /// <summary>
        /// The ID from the <see cref="Orders"/> instance.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The creation date from the <see cref="Orders"/> instance.
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// The name of the customer from the <see cref="Orders"/> instance.
        /// </summary>
        public string? CustomerID_Name { get; set; }
        /// <summary>
        /// The total (Qty * UnitPrice) from the products in the <see cref="Orders"/> instance.
        /// </summary>
        public decimal Total { get; set; }
    }

    #endregion

    #region CRUD

    /// <summary>
    /// Model representation when inserting a new entity into <see cref="Orders"/>.
    /// </summary>
    public class OrderInsertModel
    {
        /// <summary>
        /// The customer ID from the <see cref="Orders"/> instance.
        /// </summary>
        public long CustomerID { get; set; }
    }

    /// <summary>
    /// Model representation when updating a entity into <see cref="Orders"/>.
    /// </summary>
    public class OrderUpdateModel : OrderInsertModel
    {
        /// <summary>
        /// The ID from the <see cref="Orders"/> instance.
        /// </summary>
        public long ID { get; set; }
    }

    /// <summary>
    /// Model representation when returning an entity from <see cref="Orders"/>.
    /// </summary>
    public class OrderDisplayModel : OrderUpdateModel
    {
        /// <summary>
        /// The creation date from the <see cref="Orders"/> instance.
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// The name of the customer from the <see cref="Orders"/> instance.
        /// </summary>
        public string? CustomerID_Name { get; set; }
        /// <summary>
        /// The total (Qty * UnitPrice) from the products in the <see cref="Orders"/> instance.
        /// </summary>
        public decimal Total { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    /// <summary>
    /// Configure profiles for mapping <see cref="Orders"/> class with the model classes.
    /// </summary>
    public class OrderModelsProfiles : Profile
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OrderModelsProfiles"/> class.
        /// </summary>
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