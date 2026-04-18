using Application.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Core;
using Core;

namespace Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Recipe> CreateAsync(RecipeDto dto, CancellationToken ct)
    {
        var recipe = new Recipe(dto.Title, dto.Description, dto.PrepTimeMinutes, dto.CookTimeMinutes, dto.Servings, dto.Difficulty);

        foreach (var i in dto.Ingredients)
            recipe.AddIngredient(new Ingredient(i.Name, i.Quantity, i.Unit));

        foreach (var s in dto.Steps)
            recipe.AddStep(new Step(s.Instruction));

        if (!recipe.IsValid())
            throw new ArgumentException("Recipe must have at least one ingredient and one step.");

        await _repository.AddAsync(recipe, ct);
        return recipe;
    }

    public Task<IEnumerable<Recipe>> GetAllAsync(Difficulty? diff, int? maxTime, CancellationToken ct) 
        => _repository.GetAllAsync(diff, maxTime, ct);

    public Task<Recipe?> GetByIdAsync(Guid id, CancellationToken ct) 
        => _repository.GetByIdAsync(id, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct) 
        => _repository.DeleteAsync(id, ct);

    public Task<IEnumerable<Recipe>> SearchByIngredientAsync(string ing, CancellationToken ct) 
        => _repository.SearchByIngredientAsync(ing, ct);
    
    public async Task UpdateAsync(Guid id, RecipeDto dto, CancellationToken ct)
    {
        var existingRecipe = await _repository.GetByIdAsync(id, ct);
        if (existingRecipe == null)
            throw new KeyNotFoundException("Recipe not found");

        if (dto.Ingredients.Count == 0 || dto.Steps.Count == 0)
            throw new ArgumentException("Recipe must have at least one ingredient and one step.");
        
        await _repository.UpdateAsync(existingRecipe, ct); 
    }
}