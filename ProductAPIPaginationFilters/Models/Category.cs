using System;
using System.Collections.Generic;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}