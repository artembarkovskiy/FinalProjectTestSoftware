namespace Core;

public class Step
{
    public Guid Id { get; private set; }
    public Guid RecipeId { get; private set; }
    public int StepNumber { get; internal set; } 
    public string Instruction { get; private set; }
    
    public Recipe? Recipe { get; private set; }
    
    protected Step() { }

    public Step(string instruction)
    {
        if (string.IsNullOrWhiteSpace(instruction))
            throw new ArgumentException("Instruction cannot be empty", nameof(instruction));
        
        Id = Guid.NewGuid();
        Instruction = instruction;
    }
}