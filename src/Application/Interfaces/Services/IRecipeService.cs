using Application.DTOs;
using Core;

namespace Application.Interfaces.Services;

public interface IRecipeService
{
    Task<IEnumerable<Recipe>> GetAllAsync(Difficulty? difficulty, int? maxPrepTime, CancellationToken ct);
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Recipe> CreateAsync(RecipeDto dto, CancellationToken ct); 
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Recipe>> SearchByIngredientAsync(string ingredient, CancellationToken ct);
    Task UpdateAsync(Guid id, RecipeDto dto, CancellationToken ct);
}