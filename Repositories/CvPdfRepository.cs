using MainPortfolio.Models;
using MongoDB.Driver;

namespace MainPortfolio.Repositories;

public class CvPdfRepository
{
    private readonly IMongoCollection<PdfDocument> _collection;

    public CvPdfRepository(IMongoDatabase database) { _collection = database.GetCollection<PdfDocument>("cv"); }

    public async Task AddDocumentAsync(PdfDocument document) => await _collection.InsertOneAsync(document);

    public async Task<PdfDocument?> GetDocumentAsync() => await _collection.Find(_ => true).FirstOrDefaultAsync();

    public async Task DeleteAllDocumentsAsync() => await _collection.DeleteManyAsync(_ => true);
}

