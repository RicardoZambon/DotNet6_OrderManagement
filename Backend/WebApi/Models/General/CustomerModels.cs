using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.General;

namespace Zambon.OrderManagement.WebApi.Models.General
{
    #region List

    /// <summary>
    /// Model representation when returning a list of <see cref="Customers"/>.
    /// </summary>
    public class CustomersListModel
    {
        /// <summary>
        /// The ID from the <see cref="Customers"/> instance.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The name from the <see cref="Customers"/> instance.
        /// </summary>
        public string? Name { get; set; }
    }

    #endregion

    #region CRUD

    /// <summary>
    /// Model representation when inserting a new entity into <see cref="Customers"/>.
    /// </summary>
    public class CustomerInsertModel
    {
        /// <summary>
        /// The name from the <see cref="Customers"/> instance.
        /// </summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// Model representation when returning or updating a entity into <see cref="Customers"/>.
    /// </summary>
    public class CustomerUpdateModel : CustomerInsertModel
    {
        /// <summary>
        /// The ID from the <see cref="Customers"/> instance.
        /// </summary>
        public long ID { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    /// <summary>
    /// Configure profiles for mapping <see cref="Customers"/> class with the model classes.
    /// </summary>
    public class CustomerModelsProfiles : Profile
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomerModelsProfiles"/> class.
        /// </summary>
        public CustomerModelsProfiles()
        {
            CreateMap<Customers, CustomersListModel>();

            CreateMap<CustomerInsertModel, Customers>();
            CreateMap<CustomerUpdateModel, Customers>()
                .ReverseMap();
        }
    }

    #endregion
}