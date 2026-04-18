namespace Core;

public class Ingredient
{
    public Guid Id { get; private set; }
    public Guid RecipeId { get; private set; }
    public string Name { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; }
    
    public Recipe? Recipe { get; private set; }
    
    protected Ingredient() { }

    public Ingredient(string name, decimal quantity, string unit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ingredient name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));

        if (quantity <= 0) 
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Id = Guid.NewGuid();
        Name = name;
        Quantity = quantity;
        Unit = unit;
    }
}