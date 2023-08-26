using AutoMapper;
using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.WebApi.Models.Security
{
    #region List

    /// <summary>
    /// Model representation when returning a list of <see cref="Users"/>.
    /// </summary>
    public class UsersListModel
    {
        /// <summary>
        /// The ID from the <see cref="Users"/> instance.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The email from the <see cref="Users"/> instance.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// The name from the <see cref="Users"/> instance.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The username from the <see cref="Users"/> instance.
        /// </summary>
        public string? Username { get; set; }
    }

    #endregion

    #region CRUD

    /// <summary>
    /// Abstract model representation with shared properties to use when inserting or updating <see cref="Users"/>.
    /// </summary>
    public abstract class UserBaseModel {
        /// <summary>
        /// The email from the <see cref="Users"/> instance.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// The name from the <see cref="Users"/> instance.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The username from the <see cref="Users"/> instance.
        /// </summary>
        public string? Username { get; set; }
    }

    /// <summary>
    /// Model representation when inserting a new entity into <see cref="Users"/>.
    /// </summary>
    public class UserInsertModel : UserBaseModel
    {
        /// <summary>
        /// The password from the <see cref="Users"/> instance.
        /// </summary>
        public string? Password { get; set; }
    }

    /// <summary>
    /// Model representation when returning or updating a entity into <see cref="Users"/>.
    /// </summary>
    public class UserUpdateModel : UserBaseModel
    {
        /// <summary>
        /// The ID from the <see cref="Users"/> instance.
        /// </summary>
        public long ID { get; set; }
    }

    #endregion

    #region Auto Mapper Profiles

    /// <summary>
    /// Configure profiles for mapping <see cref="Users"/> class with the model classes.
    /// </summary>
    public class UserModelsProfiles : Profile
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UserModelsProfiles"/> class.
        /// </summary>
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