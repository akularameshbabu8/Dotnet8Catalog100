using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main(string[] args)
    {
       

        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddDbContext<CatalogDbContext>()
            .AddTransient<CsvImporter>()
            .BuildServiceProvider();

        var dbContext = serviceProvider.GetRequiredService<CatalogDbContext>();
        dbContext.Database.Migrate();

        var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

        string csvFilePath = config["CsvSettings:FilePath"];
        

        var importer = serviceProvider.GetRequiredService<CsvImporter>();
        importer.ImportCsv(csvFilePath);
    }
}
