using Azure;
using Azure.Data.Tables;
using System;

namespace AzureRetailApp.Models
{
    public class ProductEntity : ITableEntity
    {
        // Table keys
        public string PartitionKey { get; set; } = "Retail";
        public string RowKey { get; set; } = Guid.NewGuid().ToString("n");
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Product columns
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // IMPORTANT: Azure Table Storage supports Double (not decimal)
        public double Price { get; set; }
    }
}
