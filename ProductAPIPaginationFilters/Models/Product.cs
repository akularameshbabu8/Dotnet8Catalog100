public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CategoryCode { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}
