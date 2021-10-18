using Catalog.API.Data;
using Catalog.API.Entitites;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository:IProductRepository
    {
        private readonly ICatalogContext _context;
        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        #region CREATE
        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }
        #endregion CREATE

        #region READ
        public async Task<Product> GetProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            return await _context
                              .Products
                              .Find(p => p.Id == id)
                              .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, category);
            return await _context
                              .Products
                              .Find(filter)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);
            return await _context
                             .Products
                             .Find(filter)
                             .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context
                              .Products
                              .Find(prop => true)
                              .ToListAsync();
        }
        #endregion READ

        #region UPDATE
        public async Task<bool> UpdateProduct(Product product)
        {
            var updateResult = await _context
                                         .Products
                                         .ReplaceOneAsync<Product>(filter: p => p.Id == product.Id, replacement: product);

                return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
        #endregion UPDATE

        #region DELETE
        public async Task<bool> DeleteProduct(string id)
        {
            var deleteResult = await _context
                                    .Products
                                    .DeleteOneAsync(filter: p => p.Id == id);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;

        }
        #endregion DELETE

    }
}
