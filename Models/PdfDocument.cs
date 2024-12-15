using MongoDB.Bson;

namespace MainPortfolio.Models;

public class PdfDocument
{
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public byte[]? Content { get; set; }
    public DateTime UploadedAt { get; set; }
}