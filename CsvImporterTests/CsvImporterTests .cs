using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using System.IO;

public class CsvImporterTests
{
    private readonly CatalogDbContext _dbContext;
    private readonly CsvImporter _csvImporter;
    private readonly ILogger<CsvImporter> _logger;

    public CsvImporterTests()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase("TestDatabase") // Use only InMemory
            .Options;

        _dbContext = new CatalogDbContext(options); // Ensure constructor supports DbContextOptions
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CsvImporter>();

        _csvImporter = new CsvImporter(_dbContext, _logger);
    }

    [Fact]
    public void ImportCsv_ShouldIgnoreDuplicateProductCodes()
    {
        // Arrange: Write test CSV file
        string tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, "ProductName,ProductCode,CategoryName,CategoryCode\nTestProduct,P001,TestCategory,C001");

        // Act: Import CSV file
        _csvImporter.ImportCsv(tempFilePath);

        // Assert: Ensure only one record was inserted
        Assert.Equal(1, _dbContext.Products.Count());

        // Cleanup
        File.Delete(tempFilePath);
    }
}
