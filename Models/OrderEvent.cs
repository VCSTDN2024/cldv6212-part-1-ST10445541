namespace AzureRetailApp.Models
{
    public class OrderEvent
    {
        public string Type { get; set; } = string.Empty;     // e.g. "ImageUpload"
        public string Message { get; set; } = string.Empty;  // e.g. "Uploading file..."
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
