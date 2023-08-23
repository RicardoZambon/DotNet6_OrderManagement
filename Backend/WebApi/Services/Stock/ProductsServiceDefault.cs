using AutoMapper;
using AutoMapper.QueryableExtensions;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Stock
{
    public class ProductsServiceDefault : IProductsService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductsRepository productsRepository;

        public ProductsServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            IProductsRepository productsRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productsRepository = productsRepository;
        }


        public async Task<ProductUpdateModel> FindProductByIdAsync(long productId)
        {
            return mapper.Map<ProductUpdateModel>(await productsRepository.FindByIdAsync(productId));
        }

        public async Task<ProductUpdateModel> InsertNewProductAsync(ProductInsertModel productModel)
        {
            var product = mapper.Map<Products>(productModel);

            await productsRepository.AddAsync(product);
            await dbContext.SaveChangesAsync();

            return mapper.Map<ProductUpdateModel>(product);
        }

        public IEnumerable<ProductsListModel> ListProducts(IListParameters parameters)
        {
            return productsRepository.List(parameters).ProjectTo<ProductsListModel>(mapper.ConfigurationProvider);
        }

        public async Task RemoveProductsAsync(long[] productIds)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var product in productIds)
                {
                    await productsRepository.RemoveAsync(product);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task<ProductUpdateModel> UpdateExistingProductAsync(ProductUpdateModel productModel)
        {
            if (await productsRepository.FindByIdAsync(productModel.ID) is not Products product)
            {
                throw new EntityNotFoundException(nameof(Users), productModel.ID);
            }

            await productsRepository.UpdateAsync(mapper.Map(productModel, product));
            await dbContext.SaveChangesAsync();

            return mapper.Map<ProductUpdateModel>(product);
        }
    }
}