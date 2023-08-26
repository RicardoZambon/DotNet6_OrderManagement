using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Helpers;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Security;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace Zambon.OrderManagement.WebApi.Controllers.Security
{
    /// <summary>
    /// Controller for viewing and updating the <see cref="Users"/>.
    /// </summary>
    [ApiController, Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="usersService">The <see cref="IUsersService"/> instance.</param>
        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }


        #region List

        /// <summary>
        /// Return a list of users.
        /// </summary>
        /// <param name="parameters">Parameter object for pagination and filtering the results.</param>
        /// <returns>The <see cref="OkObjectResult"/> response with a list of <see cref="UsersListModel"/> results.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /List
        ///     {
        ///         "EndRow": 100,
        ///         "Filters": {
        ///             "Username": "username value to search"
        ///         },
        ///         "StartRow": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully returned the users list.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(UsersListModel), 200)]
        public IActionResult List([FromBody] ListParametersModel parameters)
        {
            try
            {
                return Ok(usersService.ListUsers(parameters));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException is Exception innerEx ? " " + innerEx.Message : ""));
            }
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Return a user by the ID.
        /// </summary>
        /// <param name="userId">The ID of the user to search for.</param>
        /// <returns>The <see cref="OkObjectResult"/> response with the <see cref="UserUpdateModel"/> instance.</returns>
        /// <response code="200">Sucessfully returned the user.</response>
        /// <response code="404">The user ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserUpdateModel), 200)]
        public async Task<IActionResult> Get([FromRoute] long userId)
        {
            try
            {
                return Ok(await usersService.FindUserByIdAsync(userId));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException is Exception innerEx ? " " + innerEx.Message : ""));
            }
        }

        /// <summary>
        /// Validate and add a new user.
        /// </summary>
        /// <param name="model">The user model to be inserted.</param>
        /// <returns>The <see cref="OkObjectResult"/> response with the <see cref="UserUpdateModel"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /Add
        ///     {
        ///         "Email": "email@company.com",
        ///         "Name": "John Doe",
        ///         "Password": "passsword",
        ///         "Username", "john.doe"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully inserted the user.</response>
        /// <response code="400">The user has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPut]
        [ProducesResponseType(typeof(UserUpdateModel), 200)]
        public async Task<IActionResult> Add([FromBody] UserInsertModel model)
        {
            try
            {
                return Ok(await usersService.InsertNewUserAsync(model));
            }
            catch (EntityValidationFailureException validationEx)
            {
                return ValidationProblem(new ValidationProblemEntityDetails(validationEx));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException is Exception innerEx ? " " + innerEx.Message : ""));
            }
        }
        /// <summary>
        /// Validate and update an existing user.
        /// </summary>
        /// <param name="model">The user model to be updated.</param>
        /// <returns>The <see cref="OkObjectResult"/> response with the <see cref="UserUpdateModel"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Update
        ///     {
        ///         "Email": "email@company.com",
        ///         "ID": 1,
        ///         "Name": "John Doe",
        ///         "Password": "passsword",
        ///         "Username", "john.doe"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully updated the user.</response>
        /// <response code="400">The user has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserUpdateModel), 200)]
        public async Task<IActionResult> Update([FromBody] UserUpdateModel model)
        {
            try
            {
                return Ok(await usersService.UpdateExistingUserAsync(model));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (EntityValidationFailureException validationEx)
            {
                return ValidationProblem(new ValidationProblemEntityDetails(validationEx));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException is Exception innerEx ? " " + innerEx.Message : ""));
            }
        }
        /// <summary>
        /// Delete existing users.
        /// </summary>
        /// <param name="userIds">The user IDs to be deleted.</param>
        /// <returns>The <see cref="OkResult"/> response.</returns>
        /// <response code="200">Sucessfully deleted the user IDs.</response>
        /// <response code="404">The user ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long[] userIds)
        {
            try
            {
                await usersService.RemoveUsersAsync(userIds);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException is Exception innerEx ? " " + innerEx.Message : ""));
            }
        }

        #endregion
    }
}