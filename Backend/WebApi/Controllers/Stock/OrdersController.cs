using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Helpers;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace Zambon.OrderManagement.WebApi.Controllers.Stock
{
    /// <summary>
    /// Controller for viewing and updating the <see cref="Orders"/>.
    /// </summary>
    [ApiController, Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService ordersService;
        private readonly IOrdersProductsService ordersProductsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="ordersService">The <see cref="IOrdersService"/> instance.</param>
        /// <param name="ordersProductsService">The <see cref="IOrdersProductsService"/> instance.</param>
        public OrdersController(
            IOrdersService ordersService,
            IOrdersProductsService ordersProductsService
            )
        {
            this.ordersService = ordersService;
            this.ordersProductsService = ordersProductsService;
        }


        #region List

        /// <summary>
        /// Return a list of orders.
        /// </summary>
        /// <param name="parameters">Parameter object for pagination and filtering the results.</param>
        /// <returns>A list of orders accordingly to the criteria in the parameters.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /List
        ///     {
        ///         "EndRow": 100,
        ///         "Filters": {
        ///             "CustomerID": 1
        ///         },
        ///         "StartRow": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully returned the orders list.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(OrdersListModel), 200)]
        public IActionResult List([FromBody] ListParametersModel parameters)
        {
            try
            {
                return Ok(ordersService.ListOrders(parameters));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException is Exception innerEx ? " " + innerEx.Message : ""));
            }
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Return a order by the ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <returns>An object representing the <see cref="Orders"/> instance.</returns>
        /// <response code="200">Sucessfully returned the order.</response>
        /// <response code="404">The order ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(OrderDisplayModel), 200)]
        public async Task<IActionResult> Get([FromRoute] long orderId)
        {
            try
            {
                return Ok(await ordersService.FindOrderByIdAsync(orderId));
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
        /// Return the order total by the ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <returns>The total (sum of the products Qty * UnitPrice) of the <see cref="Orders"/> instance.</returns>
        /// <response code="200">Sucessfully returned the order total.</response>
        /// <response code="404">The order ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpGet("{orderId}/[action]")]
        public async Task<IActionResult> Total([FromRoute] long orderId)
        {
            try
            {
                return Ok(await ordersService.GetOrderTotalAsync(orderId));
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
        /// Validate and add a new order.
        /// </summary>
        /// <param name="model">The order model to be inserted.</param>
        /// <returns>An object representing the <see cref="Orders"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /Add
        ///     {
        ///         "CustomerID": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully inserted the order.</response>
        /// <response code="400">The order has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPut]
        [ProducesResponseType(typeof(OrderDisplayModel), 200)]
        public async Task<IActionResult> Add([FromBody] OrderInsertModel model)
        {
            try
            {
                return Ok(await ordersService.InsertNewOrderAsync(model)); ;
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
        /// Validate and update an existing order.
        /// </summary>
        /// <param name="model">The order model to be updated.</param>
        /// <returns>An object representing the <see cref="Orders"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Update
        ///     {
        ///         "ID": 1,
        ///         "CustomerID": 2
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully updated the order.</response>
        /// <response code="400">The order has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDisplayModel), 200)]
        public async Task<IActionResult> Update([FromBody] OrderUpdateModel model)
        {
            try
            {
                return Ok(await ordersService.UpdateExistingOrderAsync(model));
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
        /// Delete existing orders.
        /// </summary>
        /// <param name="orderIds">The order IDs to be deleted.</param>
        /// <returns>Async task result indicating the job completion.</returns>
        /// <response code="200">Sucessfully deleted the order IDs.</response>
        /// <response code="404">The order ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long[] orderIds)
        {
            try
            {
                await ordersService.RemoveOrdersAsync(orderIds);
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

        #region Products

        /// <summary>
        /// Return a list of products from an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <param name="parameters">Parameter object for pagination and filtering the results.</param>
        /// <returns>A list of products from an order accordingly to the criteria in the parameters.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /List
        ///     {
        ///         "EndRow": 100,
        ///         "StartRow": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully returned the list of products in the order.</response>
        /// <response code="404">The order ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost("{orderId}/[action]/List")]
        [ProducesResponseType(typeof(OrdersProductsListModel), 200)]
        public async Task<IActionResult> Products([FromRoute] long orderId, [FromBody] ListParametersModel parameters)
        {
            try
            {
                return Ok(await ordersProductsService.ListOrdersProductsAsync(orderId, parameters));
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
        /// Update the products of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <param name="model">Parameter object with products to be inserted, updated, or deleted from the order.</param>
        /// <returns>Async task result indicating the job completion.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /List
        ///     {
        ///         "EntitiesToAddUpdate": [
        ///             { "ID": 1, "ProductID": 1, "Qty": 15 },
        ///             { "ID": 2, "ProductID": 2, "Qty": 7 },
        ///             { "ID": 0, "ProductID": 1, "Qty": 1 }
        ///         ],
        ///         "EntitiesToDelete": [ 3, 4 ]
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully deleted the products in the order.</response>
        /// <response code="400">One of the products in the order has validation issues, check response.</response>
        /// <response code="404">The order ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost("{orderId}/[action]")]
        [ProducesResponseType(typeof(OrdersProductUpdateModel), 200)]
        public async Task<IActionResult> Products([FromRoute] long orderId, [FromBody] BatchUpdateModel<OrdersProductUpdateModel, long> model)
        {
            try
            {
                await ordersProductsService.BatchUpdateOrdersProductsAsync(orderId, model);
                return Ok();
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

        #endregion
    }
}