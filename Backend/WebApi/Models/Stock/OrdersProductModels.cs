using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;

namespace Zambon.OrderManagement.WebApi.Models.Stock
{
    #region List

    /// <summary>
    /// Model representation when returning a list of <see cref="OrdersProducts"/>.
    /// </summary>
    public class OrdersProductsListModel
    {
        /// <summary>
        /// The ID from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The product ID from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public long ProductID { get; set; }
        /// <summary>
        /// The product name the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public string? ProductID_Name { get; set; }
        /// <summary>
        /// The qty from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// The unit price from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// The total (<see cref="Qty"/> * <see cref="UnitPrice"/>) from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public decimal Total { get; set; }
    }

    #endregion

    #region CRUD

    /// <summary>
    /// Model representation when inserting or updating an entity into <see cref="OrdersProducts"/>.
    /// </summary>
    public class OrdersProductUpdateModel
    {
        /// <summary>
        /// The ID from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The product ID from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public long ProductID { get; set; }
        /// <summary>
        /// The qty from the <see cref="OrdersProducts"/> instance.
        /// </summary>
        public int Qty { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    /// <summary>
    /// Configure profiles for mapping <see cref="OrdersProducts"/> class with the model classes.
    /// </summary>
    public class OrdersProductModelsProfiles : Profile
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OrdersProductModelsProfiles"/> class.
        /// </summary>
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