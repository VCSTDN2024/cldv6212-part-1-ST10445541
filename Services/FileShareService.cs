using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace AzureRetailApp.Services
{
    public class FileShareService
    {
        private readonly ShareDirectoryClient _dir;

        // 2-arg ctor: connection string + share name
        public FileShareService(string connectionString, string shareName)
        {
            var share = new ShareClient(connectionString, shareName);
            share.CreateIfNotExists();
            _dir = share.GetRootDirectoryClient();
            _dir.CreateIfNotExists();
        }

        public IEnumerable<string> List(int take = 200) =>
            _dir.GetFilesAndDirectories()
                .Where(i => !i.IsDirectory)
                .Take(take)
                .Select(i => i.Name);

        // Create -> SetHeaders -> UploadRange
        public async Task UploadAsync(Stream stream, string fileName, string? contentType = null)
        {
            var file = _dir.GetFileClient(fileName);

            // Ensure known size
            Stream upload = stream;
            long size;
            if (stream.CanSeek)
            {
                stream.Position = 0;
                size = stream.Length;
            }
            else
            {
                var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                upload = ms;
                size = ms.Length;
            }

            await file.DeleteIfExistsAsync();
            await file.CreateAsync(maxSize: size);

            if (!string.IsNullOrWhiteSpace(contentType))
            {
                var headers = new ShareFileHttpHeaders { ContentType = contentType };
                await file.SetHttpHeadersAsync(httpHeaders: headers);
            }

            await file.UploadRangeAsync(range: new HttpRange(0, size), content: upload);
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            var file = _dir.GetFileClient(fileName);
            var resp = await file.DownloadAsync();
            return resp.Value.Content;
        }
    }
}
