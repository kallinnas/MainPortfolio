using Microsoft.AspNetCore.Mvc;

using MainPortfolio.Repositories;
using MainPortfolio.Models;

namespace MainPortfolio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly CvPdfRepository _repository;
    public FileController(CvPdfRepository repository) { _repository = repository; }

    [HttpPost("UploadCv")]
    public async Task<IActionResult> UploadCv([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var document = new PdfDocument
        {
            Name = file.FileName,
            Type = file.ContentType,
            Content = memoryStream.ToArray(),
            UploadedAt = DateTime.UtcNow
        };

        await _repository.AddDocumentAsync(document);
        return Ok();
    }

    [HttpGet("getCv")]
    public async Task<IActionResult> GetCv()
    {
        var document = await _repository.GetDocumentAsync();
        if (document == null)
            return NotFound("No document found.");

        return File(document.Content!, document.Type!, document.Name);
    }

    [HttpDelete("DeleteCvs")]
    public async Task<IActionResult> DeleteCvs()
    {
        await _repository.DeleteAllDocumentsAsync();
        return Ok();
    }
}
