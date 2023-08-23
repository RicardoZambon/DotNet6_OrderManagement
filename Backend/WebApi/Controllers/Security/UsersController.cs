using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Helpers;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Security;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace Zambon.OrderManagement.WebApi.Controllers.Security
{
    [ApiController, Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService usersService;


        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        #region List

        [HttpPost("[action]")]
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

        [HttpGet("{userId}")]
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

        [HttpPut]
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
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UserUpdateModel model)
        {
            try
            {
                return Ok(await usersService.UpdateExistingCustomerAsync(model));
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
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long[] userIds)
        {
            try
            {
                await usersService.RemoveCustomersAsync(userIds);
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