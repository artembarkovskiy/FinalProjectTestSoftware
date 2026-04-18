using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;

namespace Application.Interfaces.Repositories;

public interface IRecipeRepository
{

    Task<IEnumerable<Recipe>> GetAllAsync(
        Difficulty? difficulty = null, 
        int? maxPrepTimeMinutes = null, 
        CancellationToken cancellationToken = default);
    
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Recipe>> SearchByIngredientAsync(string ingredientName, CancellationToken cancellationToken = default);
}