using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.DTOs;
using AutoFixture;
using Core;
using FluentAssertions;
using Xunit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Integration;

public class RecipeApiTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly IntegrationTestWebAppFactory _factory;

    public RecipeApiTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateRecipe_WithValidNestedData_ShouldReturnCreatedAndSaveToDb()
    {
        // Arrange
        var newRecipe = new
        {
            title = _fixture.Create<string>(),
            description = _fixture.Create<string>(),
            prepTimeMinutes = 15,
            cookTimeMinutes = 45,
            servings = 4,
            difficulty = 2,
            ingredients = new[]
            {
                new { name = _fixture.Create<string>(), quantity = 2.5m, unit = "kg" }
            },
            steps = new[]
            {
                new { instruction = _fixture.Create<string>() }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/recipes", newRecipe);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonResponse = System.Text.Json.Nodes.JsonNode.Parse(responseString);
        
        jsonResponse.Should().NotBeNull();
        jsonResponse["title"].GetValue<string>().Should().Be(newRecipe.title);
        jsonResponse["ingredients"].AsArray().Should().HaveCount(1);
        jsonResponse["steps"].AsArray().Should().HaveCount(1);
    }
    
    [Fact]
    public async Task DeleteRecipe_ShouldCascadeDeleteIngredientsAndSteps_FromDatabase()
    {
        var newRecipe = new RecipeDto
        {
            Title = _fixture.Create<string>(),
            Description = _fixture.Create<string>(),
            PrepTimeMinutes = 10,
            CookTimeMinutes = 20,
            Servings = 2,
            Difficulty = Difficulty.Easy,
            Ingredients = new List<IngredientDto> 
            { 
                new() { Name = _fixture.Create<string>(), Quantity = 1, Unit = "шт" } 
            },
            Steps = new List<StepDto> 
            { 
                new() { Instruction = _fixture.Create<string>() } 
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/recipes", newRecipe);
        var createdRecipeStr = await createResponse.Content.ReadAsStringAsync();
        var recipeId = System.Text.Json.Nodes.JsonNode.Parse(createdRecipeStr)!["id"]!.GetValue<Guid>();
        
        var deleteResponse = await _client.DeleteAsync($"/api/recipes/{recipeId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var recipeExists = await dbContext.Recipes.AnyAsync(r => r.Id == recipeId);
        recipeExists.Should().BeFalse();


        var ingredientsExist = await dbContext.Ingredients.AnyAsync(i => i.RecipeId == recipeId);
        var stepsExist = await dbContext.Steps.AnyAsync(s => s.RecipeId == recipeId);

        ingredientsExist.Should().BeFalse("тому що спрацювало каскадне видалення інгредієнтів");
        stepsExist.Should().BeFalse("тому що спрацювало каскадне видалення кроків");
    }
    
    [Fact]
    public async Task GetById_WhenRecipeDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var randomId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/recipes/{randomId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WhenRecipeExists_ShouldReturnOkAndRecipeDetails()
    {
        // Arrange
        var newRecipe = new RecipeDto
        {
            Title = _fixture.Create<string>(),
            Description = _fixture.Create<string>(),
            PrepTimeMinutes = 10,
            CookTimeMinutes = 20,
            Servings = 2,
            Difficulty = Difficulty.Easy,
            Ingredients = new List<IngredientDto> { new() { Name = "Test Ingredient", Quantity = 1, Unit = "kg" } },
            Steps = new List<StepDto> { new() { Instruction = "Test Step" } }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", newRecipe);
        var createdStr = await createResponse.Content.ReadAsStringAsync();
        var recipeId = System.Text.Json.Nodes.JsonNode.Parse(createdStr)!["id"]!.GetValue<Guid>();

        // Act
        var getResponse = await _client.GetAsync($"/api/recipes/{recipeId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getStr = await getResponse.Content.ReadAsStringAsync();
        var fetchedRecipe = System.Text.Json.Nodes.JsonNode.Parse(getStr)!;
        
        fetchedRecipe["title"]!.GetValue<string>().Should().Be(newRecipe.Title);
        fetchedRecipe["ingredients"]!.AsArray().Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkAndListOfRecipes()
    {
        // Act
        var response = await _client.GetAsync("/api/recipes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonArray = System.Text.Json.Nodes.JsonArray.Parse(responseString) as System.Text.Json.Nodes.JsonArray;
        
        jsonArray.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchByIngredient_WithMatchingIngredient_ShouldReturnRecipe()
    {
        // Arrange
        var uniqueIngredient = "UnicornTears_" + Guid.NewGuid().ToString().Substring(0, 5);
        var newRecipe = new RecipeDto
        {
            Title = "Magic Potion",
            Description = "Very magical",
            PrepTimeMinutes = 5,
            CookTimeMinutes = 5,
            Servings = 1,
            Difficulty = Difficulty.Hard,
            Ingredients = new List<IngredientDto> { new() { Name = uniqueIngredient, Quantity = 1, Unit = "drop" } },
            Steps = new List<StepDto> { new() { Instruction = "Mix well" } }
        };
        await _client.PostAsJsonAsync("/api/recipes", newRecipe);

        // Act:
        var response = await _client.GetAsync($"/api/recipes/search?ingredient={uniqueIngredient}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonArray = System.Text.Json.Nodes.JsonArray.Parse(responseString) as System.Text.Json.Nodes.JsonArray;
        
        jsonArray.Should().NotBeNull();
        jsonArray!.Count.Should().BeGreaterThanOrEqualTo(1); 
        jsonArray[0]!["title"]!.GetValue<string>().Should().Be("Magic Potion");
    }
}