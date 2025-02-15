public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Primary Key
    public string Name { get; set; }
    public string Code { get; set; } // Unique identifier for foreign key
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
