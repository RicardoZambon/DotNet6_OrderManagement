using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.General;

namespace Zambon.OrderManagement.WebApi.Models.General
{
    #region List

    public class CustomersListModel
    {
        public long ID { get; set; }
        public string? Name { get; set; }
    }

    #endregion

    #region CRUD

    public class CustomerInsertModel
    {
        public string? Name { get; set; }
    }

    public class CustomerUpdateModel : CustomerInsertModel
    {
        public long ID { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    public class CustomerModelsProfiles : Profile
    {
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