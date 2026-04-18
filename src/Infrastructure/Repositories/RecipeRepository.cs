using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;
using Core;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _context;

    public RecipeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync(Difficulty? difficulty = null, int? maxPrepTimeMinutes = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Recipes.AsNoTracking();

        if (difficulty.HasValue)
            query = query.Where(r => r.Difficulty == difficulty.Value);

        if (maxPrepTimeMinutes.HasValue)
            query = query.Where(r => r.PrepTimeMinutes <= maxPrepTimeMinutes.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        await _context.Recipes.AddAsync(recipe, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes.FindAsync(new object[] { id }, cancellationToken);
        if (recipe != null)
        {
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Recipe>> SearchByIngredientAsync(string ingredientName, CancellationToken cancellationToken = default)
    {
        var searchTerm = ingredientName.ToLower();
        
        return await _context.Recipes
            .AsNoTracking()
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Where(r => r.Ingredients.Any(i => i.Name.ToLower().Contains(searchTerm)))
            .ToListAsync(cancellationToken);
    }
}