using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

public class CsvImporter
{
    private readonly CatalogDbContext _dbContext;
    private readonly ILogger<CsvImporter> _logger;

    public CsvImporter(CatalogDbContext dbContext, ILogger<CsvImporter> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public void ImportCsv(string filePath)
    {
        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.Trim().Replace(" ", "")
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<ProductCsvModel>().ToList();

            _logger.LogInformation($"Read {records.Count} records from CSV.");

            var categoryDictionary = _dbContext.Categories.ToDictionary(c => c.Code, c => c);
            var existingProductCodes = new HashSet<string>(
                _dbContext.Products.Select(p => p.Code).ToList(),
                StringComparer.OrdinalIgnoreCase
            );

            foreach (var record in records)
            {
                // Ensure category exists
                if (!categoryDictionary.TryGetValue(record.CategoryCode, out var category))
                {
                    category = _dbContext.Categories
                        .AsNoTracking() // Avoid tracking multiple instances
                        .FirstOrDefault(c => c.Code.Equals(record.CategoryCode, StringComparison.OrdinalIgnoreCase));

                    if (category == null)
                    {
                        category = new Category
                        {
                            Id = Guid.NewGuid(),
                            Name = record.CategoryName,
                            Code = record.CategoryCode,
                            CreationDate = DateTime.UtcNow
                        };
                        _dbContext.Categories.Add(category);
                    }
                    else
                    {
                        _dbContext.Attach(category);
                    }

                    categoryDictionary[record.CategoryCode] = category;
                }

                // Ensure product is unique
                if (!existingProductCodes.Contains(record.ProductCode))
                {
                    _dbContext.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = record.ProductName,
                        Code = record.ProductCode,
                        Category = category,
                        CreationDate = DateTime.UtcNow
                    });

                    existingProductCodes.Add(record.ProductCode); // Prevent duplicate inserts
                }
                else
                {
                    _logger.LogWarning($"Duplicate Product Code Skipped: {record.ProductCode}");
                }
            }

            _dbContext.SaveChanges();
            _logger.LogInformation("CSV import completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error importing CSV: {ex.Message}", ex);
        }
    }
}

public class ProductCsvModel
{
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string CategoryName { get; set; }
    public string CategoryCode { get; set; }
}
