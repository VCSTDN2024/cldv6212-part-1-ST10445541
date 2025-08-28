using System;

namespace AzureRetailApp.Models
{
    // Singular type name you asked for: QueuesEvent
    public record QueuesEvent(
        string Type,
        string Id,
        string Description,
        DateTimeOffset Timestamp);
}
