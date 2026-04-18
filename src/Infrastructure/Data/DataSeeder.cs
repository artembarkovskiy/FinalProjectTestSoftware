using Bogus;
using Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Recipes.AnyAsync())
        {
            return;
        }
        
        Randomizer.Seed = new Random(8675309);
        
        var ingredientFaker = new Faker<Ingredient>()
            .CustomInstantiator(f => new Ingredient(
                f.Commerce.ProductName(),
                Math.Round(f.Random.Decimal(0.5m, 5m), 1),
                f.PickRandom("g", "kg", "ml", "tbsp", "tsp", "pieces")
            ));
        
        var stepFaker = new Faker<Step>()
            .CustomInstantiator(f => new Step(
                f.Lorem.Sentence(wordCount: 5)
            ));
        
        var recipeFaker = new Faker<Recipe>()
            .CustomInstantiator(f => 
            {
                var recipe = new Recipe(
                    $"Recipe: {f.Commerce.ProductName()}",
                    f.Lorem.Paragraph(),
                    f.Random.Int(5, 60),
                    f.Random.Int(10, 120),
                    f.Random.Int(1, 8),
                    f.PickRandom<Difficulty>()
                );
                
                var ingredientsCount = f.Random.Int(2, 6);
                for (int i = 0; i < ingredientsCount; i++)
                {
                    recipe.AddIngredient(ingredientFaker.Generate());
                }
                
                var stepsCount = f.Random.Int(3, 7);
                for (int i = 0; i < stepsCount; i++)
                {
                    recipe.AddStep(stepFaker.Generate());
                }

                return recipe;
            });
        
        var recipes = recipeFaker.Generate(2000);
        
        await _context.Recipes.AddRangeAsync(recipes);
        await _context.SaveChangesAsync();
    }
}