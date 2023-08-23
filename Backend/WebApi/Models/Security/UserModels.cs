using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.WebApi.Models.Security
{
    #region List

    public class UsersListModel
    {
        public long ID { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
    }

    #endregion

    #region CRUD

    public abstract class UserBaseModel {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
    }

    public class UserInsertModel : UserBaseModel
    {
        public string? Password { get; set; }
    }

    public class UserUpdateModel : UserBaseModel
    {
        public long ID { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    public class UserModelsProfiles : Profile
    {
        public UserModelsProfiles()
        {
            CreateMap<Users, UsersListModel>();

            CreateMap<UserInsertModel, Users>();
            CreateMap<UserUpdateModel, Users>()
                .ReverseMap();
        }
    }

    #endregion
}