using Azure.Data.Tables;
using AzureRetailApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureRetailApp.Services
{
    public class TableStorageService
    {
        private readonly TableClient _tableClient;

        public TableStorageService(string connectionString, string tableName = "Customers")
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task AddCustomerAsync(CustomerEntity customer)
        {
            await _tableClient.AddEntityAsync(customer);
        }

        public async Task<List<CustomerEntity>> GetCustomersAsync()
        {
            var customers = new List<CustomerEntity>();
            await foreach (var entity in _tableClient.QueryAsync<CustomerEntity>())
            {
                customers.Add(entity);
            }
            return customers;
        }

        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        // 👇 New method to make Program.cs happy
        public async Task EnsureTablesAsync()
        {
            await _tableClient.CreateIfNotExistsAsync();

            // Seed 5 customers if table is empty
            if (!_tableClient.Query<CustomerEntity>().Any())
            {
                var demoCustomers = Enumerable.Range(1, 5).Select(i => new CustomerEntity
                {
                    PartitionKey = "Retail",
                    RowKey = Guid.NewGuid().ToString("n"),
                    FirstName = $"First{i}",
                    LastName = $"Last{i}",
                    Email = $"customer{i}@example.com"
                });

                foreach (var c in demoCustomers)
                {
                    await _tableClient.AddEntityAsync(c);
                }
            }
        }
    }
}
