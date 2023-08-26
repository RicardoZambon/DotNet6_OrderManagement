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
    /// Controller for viewing and updating the <see cref="Products"/>.
    /// </summary>
    [ApiController, Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService productsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="productsService">The <see cref="IProductsService"/> instance.</param>
        public ProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }


        #region List

        /// <summary>
        /// Return a list of products.
        /// </summary>
        /// <param name="parameters">Parameter object for pagination and filtering the results.</param>
        /// <returns>A list of products accordingly to the criteria in the parameters.</returns>
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
        /// <response code="200">Sucessfully returned the products list.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost("[action]")]
        public IActionResult List([FromBody] ListParametersModel parameters)
        {
            try
            {
                return Ok(productsService.ListProducts(parameters));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + (ex.InnerException is Exception innerEx ? " " + innerEx.Message : ""));
            }
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Return a product by the ID.
        /// </summary>
        /// <param name="productId">The ID of the product to search for.</param>
        /// <returns>An object representing the <see cref="Products"/> instance.</returns>
        /// <response code="200">Sucessfully returned the product.</response>
        /// <response code="404">The product ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpGet("{productId}")]
        public async Task<IActionResult> Get([FromRoute] long productId)
        {
            try
            {
                return Ok(await productsService.FindProductByIdAsync(productId));
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
        /// Validate and add a new product.
        /// </summary>
        /// <param name="model">The product model to be inserted.</param>
        /// <returns>An object representing the <see cref="Products"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /Add
        ///     {
        ///         "Name": "Product name"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully inserted the product.</response>
        /// <response code="400">The product has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPut]
        public async Task<IActionResult> Add([FromBody] ProductInsertModel model)
        {
            try
            {
                return Ok(await productsService.InsertNewProductAsync(model)); ;
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
        /// Validate and update an existing product.
        /// </summary>
        /// <param name="model">The product model to be updated.</param>
        /// <returns>An object representing the <see cref="Products"/> instance.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Update
        ///     {
        ///         "ID": 1,
        ///         "Name": "New product name"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully updated the product.</response>
        /// <response code="400">The product has validation issues, check response.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] ProductUpdateModel model)
        {
            try
            {
                return Ok(await productsService.UpdateExistingProductAsync(model));
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
        /// Delete existing products.
        /// </summary>
        /// <param name="productIds">The product IDs to be deleted.</param>
        /// <returns>Async task result indicating the job completion.</returns>
        /// <response code="200">Sucessfully deleted the product IDs.</response>
        /// <response code="404">The product ID was not found.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long[] productIds)
        {
            try
            {
                await productsService.RemoveProductsAsync(productIds);
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