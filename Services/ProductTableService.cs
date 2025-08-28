using Azure.Data.Tables;
using AzureRetailApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureRetailApp.Services
{
    public class ProductTableService
    {
        private readonly TableClient _table;

        public ProductTableService(string connectionString, string tableName = "Products")
        {
            var svc = new TableServiceClient(connectionString);
            _table = svc.GetTableClient(tableName);
            _table.CreateIfNotExists();
        }

        public async Task<List<ProductEntity>> GetAllAsync()
        {
            var list = new List<ProductEntity>();
            await foreach (var e in _table.QueryAsync<ProductEntity>())
                list.Add(e);
            return list.OrderByDescending(x => x.Timestamp).ToList();
        }

        public Task AddAsync(ProductEntity product) => _table.AddEntityAsync(product);

        public Task DeleteAsync(string partitionKey, string rowKey) =>
            _table.DeleteEntityAsync(partitionKey, rowKey);
    }
}
