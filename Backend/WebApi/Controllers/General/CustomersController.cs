using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Helpers;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.General;
using Zambon.OrderManagement.WebApi.Services.General.Interfaces;

namespace Zambon.OrderManagement.WebApi.Controllers.General
{
    /// <summary>
    /// Controller for viewing and updating the <see cref="Customers"/>.
    /// </summary>
    [ApiController, Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService customersService;

        /// <summary>
        /// Initializes a new instance of <see cref="CustomersController"/> class.
        /// </summary>
        /// <param name="customersService">The <see cref="ICustomersService"/> instance.</param>
        public CustomersController(ICustomersService customersService)
        {
            this.customersService = customersService;
        }


        #region List

        /// <summary>
        /// Return a list of customers.
        /// </summary>
        /// <param name="parameters">Parameter object for pagination and filtering the results.</param>
        /// <returns>A list of customers accordingly to the criteria in the parameters.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /List
        ///     {
        ///         "EndRow": 100,
        ///         "Filters": {
        ///             "Name": "name value to search"
        ///         },
        ///         "StartRow": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully returned the customers list.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(CustomersListModel), 200)]
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

        /// <summary>
        /// Return a customer by the ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to search for.</param>
        /// <returns>An object representing the <see cref="Customers"/> instance.</returns>
        /// <response code="200">Sucessfully returned the customer.</response>
        /// <response code="404">The customer ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(CustomerUpdateModel), 200)]
        public async Task<IActionResult> Get([FromRoute] long customerId)
        {
            try
            {
                return Ok(await customersService.FindCustomerByIdAsync(customerId));
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
        /// Validate and add a new customer.
        /// </summary>
        /// <param name="model">The customer model to be inserted.</param>
        /// <returns>An object representing the <see cref="Customers"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /Add
        ///     {
        ///         "Name": "Customer name"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully inserted the customer.</response>
        /// <response code="400">The customer has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPut]
        [ProducesResponseType(typeof(CustomerUpdateModel), 200)]
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
        /// <summary>
        /// Validate and update an existing customer.
        /// </summary>
        /// <param name="model">The customer model to be updated.</param>
        /// <returns>An object representing the <see cref="Customers"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Update
        ///     {
        ///         "ID": 1,
        ///         "Name": "New customer name"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully updated the customer.</response>
        /// <response code="400">The customer has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerUpdateModel), 200)]
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
        /// <summary>
        /// Delete existing customers.
        /// </summary>
        /// <param name="customerIds">The customer IDs to be deleted.</param>
        /// <returns>Async task result indicating the job completion.</returns>
        /// <response code="200">Sucessfully deleted the customer IDs.</response>
        /// <response code="404">The customer ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long[] customerIds)
        {
            try
            {
                await customersService.RemoveCustomersAsync(customerIds);
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