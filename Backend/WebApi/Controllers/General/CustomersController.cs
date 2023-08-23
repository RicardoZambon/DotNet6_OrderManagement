using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Helpers;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.General;
using Zambon.OrderManagement.WebApi.Services.General.Interfaces;

namespace Zambon.OrderManagement.WebApi.Controllers.General
{
    [ApiController, Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService customersService;

        public CustomersController(ICustomersService customersService)
        {
            this.customersService = customersService;
        }

        #region List

        [HttpPost("[action]")]
        public IActionResult List([FromBody] ListParametersModel parameters)
        {
            try
            {
                return Ok(customersService.ListCustomers(parameters));
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
                return Ok(await customersService.FindCustomerByIdAsync(userId));
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
        public async Task<IActionResult> Add([FromBody] CustomerInsertModel model)
        {
            try
            {
                return Ok(await customersService.InsertNewCustomerAsync(model));
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
        public async Task<IActionResult> Update([FromBody] CustomerUpdateModel model)
        {
            try
            {
                return Ok(await customersService.UpdateExistingCustomerAsync(model));
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
                await customersService.RemoveCustomersAsync(userIds);
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