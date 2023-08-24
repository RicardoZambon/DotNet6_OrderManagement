using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Helpers;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace Zambon.OrderManagement.WebApi.Controllers.Stock
{
    [ApiController, Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService ordersService;
        private readonly IOrdersProductsService ordersProductsService;

        public OrdersController(
            IOrdersService ordersService,
            IOrdersProductsService ordersProductsService
            )
        {
            this.ordersService = ordersService;
            this.ordersProductsService = ordersProductsService;
        }

        #region List

        [HttpPost("[action]")]
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

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute] long userId)
        {
            try
            {
                return Ok(await ordersService.FindOrderByIdAsync(userId));
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
        [HttpPost]
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
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long[] userIds)
        {
            try
            {
                await ordersService.RemoveOrdersAsync(userIds);
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

        [HttpPost("{orderId}/[action]/List")]
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

        [HttpPost("{orderId}/[action]")]
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