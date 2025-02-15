public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Primary Key
    public string Name { get; set; }
    public string Code { get; set; } // Unique
    public string CategoryCode { get; set; } // Foreign Key
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public Category Category { get; set; }
}
