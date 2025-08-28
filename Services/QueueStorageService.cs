using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using AzureRetailApp.Models;

namespace AzureRetailApp.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queue;

        public QueueStorageService(string connectionString, string queueName = "retail-events")
        {
            _queue = new QueueClient(connectionString, queueName);
            _queue.CreateIfNotExists();
        }

        public Task EnqueueAsync(QueuesEvent evt)
            => _queue.SendMessageAsync(JsonSerializer.Serialize(evt));

        public async Task<IEnumerable<string>> PeekAsync(int max = 32)
        {
            var msgs = await _queue.PeekMessagesAsync(max > 32 ? 32 : max);
            return msgs.Value.Select(m => m.MessageText);
        }
    }
}
