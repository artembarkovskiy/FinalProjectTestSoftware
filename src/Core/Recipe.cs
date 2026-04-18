namespace Core;

public class Recipe
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int PrepTimeMinutes { get; private set; }
    public int CookTimeMinutes { get; private set; }
    public int Servings { get; private set; }
    public Difficulty Difficulty { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Ingredient> _ingredients = new();
    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients.AsReadOnly();

    private readonly List<Step> _steps = new();
    public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();
    
    protected Recipe() { }

    public Recipe(string title, string description, int prepTimeMinutes, int cookTimeMinutes, int servings, Difficulty difficulty)
    {
        
        if (string.IsNullOrWhiteSpace(title)) 
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (prepTimeMinutes <= 0) throw new ArgumentException("Prep time must be positive", nameof(prepTimeMinutes));
        if (cookTimeMinutes <= 0) throw new ArgumentException("Cook time must be positive", nameof(cookTimeMinutes));
        if (servings < 1) throw new ArgumentException("Servings must be at least 1", nameof(servings));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        PrepTimeMinutes = prepTimeMinutes;
        CookTimeMinutes = cookTimeMinutes;
        Servings = servings;
        Difficulty = difficulty;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient);
        _ingredients.Add(ingredient);
    }

    public void AddStep(Step step)
    {
        ArgumentNullException.ThrowIfNull(step);
        
        step.StepNumber = _steps.Count + 1; 
        
        _steps.Add(step);
    }
    
    public bool IsValid()
    {
        return _ingredients.Any() && _steps.Any();
    }
}