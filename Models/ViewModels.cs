using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureRetailApp.Models
{
    // Customers page view model
    public class CustomersListViewModel
    {
        public IEnumerable<CustomerEntity> Customers { get; set; } = Enumerable.Empty<CustomerEntity>();
    }

    // Products page view model
    public class ProductsListViewModel
    {
        public IEnumerable<ProductEntity> Products { get; set; } = Enumerable.Empty<ProductEntity>();
    }

    // Blobs page item + list view models
    public class BlobItemViewModel
    {
        public string Name { get; set; } = string.Empty;
        public Uri Uri { get; set; } = default!;
    }

    public class BlobsListViewModel
    {
        public IEnumerable<BlobItemViewModel> Blobs { get; set; } = Enumerable.Empty<BlobItemViewModel>();
    }

    // Queues page view model (peeked messages as raw strings)
    public class QueuesListViewModel
    {
        public IEnumerable<string> Messages { get; set; } = Enumerable.Empty<string>();
    }

    // Files page view model
    public class FilesListViewModel
    {
        public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();
    }
}
