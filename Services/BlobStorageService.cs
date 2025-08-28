using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureRetailApp.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(string connectionString, string containerName = "product-images")
        {
            var svc = new BlobServiceClient(connectionString);
            _container = svc.GetBlobContainerClient(containerName);
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task UploadAsync(System.IO.Stream stream, string fileName, string contentType)
        {
            var blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
        }

        public IEnumerable<(string Name, Uri Uri)> List(int take = 200)
            => _container.GetBlobs().Take(take)
                .Select(b => (b.Name, _container.GetBlobClient(b.Name).Uri));

        public async Task<System.IO.Stream> DownloadAsync(string name)
        {
            var resp = await _container.GetBlobClient(name).DownloadStreamingAsync();
            return resp.Value.Content;
        }
    }
}
