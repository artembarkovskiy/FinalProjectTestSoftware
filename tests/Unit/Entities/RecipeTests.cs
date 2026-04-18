using System;
using System.Linq;
using AutoFixture;
using Core;
using FluentAssertions;
using Xunit;

namespace Unit.Entities;

public class RecipeTests
{
    private readonly IFixture _fixture;

    public RecipeTests()
    {
        _fixture = new Fixture();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_WhenPrepTimeIsInvalid_ShouldThrowArgumentException(int invalidPrepTime)
    {
        // Act
        Action act = () => new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            invalidPrepTime,
            15,
            2,
            Difficulty.Easy);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Prep time must be positive*"); 
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenCookTimeIsInvalid_ShouldThrowArgumentException(int invalidCookTime)
    {
        // Act
        Action act = () => new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            15,
            invalidCookTime,
            2,
            Difficulty.Medium);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Cook time must be positive*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenServingsIsInvalid_ShouldThrowArgumentException(int invalidServings)
    {
        // Act
        Action act = () => new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            15,
            30,
            invalidServings,
            Difficulty.Hard);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Servings must be at least 1*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenTitleIsNullOrWhiteSpace_ShouldThrowArgumentException(string invalidTitle)
    {
        // Act
        Action act = () => new Recipe(
            invalidTitle,
            _fixture.Create<string>(),
            15,
            30,
            4,
            Difficulty.Easy);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddStep_ShouldAssignSequentialStepNumbers()
    {
        // Arrange
        var recipe = new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            10, 20, 4, Difficulty.Medium);

        var step1 = new Step("First step instruction");
        var step2 = new Step("Second step instruction");

        // Act
        recipe.AddStep(step1);
        recipe.AddStep(step2);

        // Assert
        recipe.Steps.Should().HaveCount(2);
        recipe.Steps.First().StepNumber.Should().Be(1);
        recipe.Steps.Last().StepNumber.Should().Be(2);
    }

    [Fact]
    public void AddIngredient_ShouldAddIngredientToList()
    {
        // Arrange
        var recipe = new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            10, 20, 4, Difficulty.Medium);

        var ingredient = new Ingredient("Salt", 1.5m, "tsp");

        // Act
        recipe.AddIngredient(ingredient);

        // Assert
        recipe.Ingredients.Should().HaveCount(1);
        recipe.Ingredients.First().Name.Should().Be("Salt");
    }

    [Fact]
    public void IsValid_WhenHasIngredientAndStep_ShouldReturnTrue()
    {
        // Arrange
        var recipe = new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            15, 30, 2, Difficulty.Hard);

        recipe.AddIngredient(new Ingredient("Salt", 1, "tsp"));
        recipe.AddStep(new Step("Add salt to the boiling water"));

        // Act
        var isValid = recipe.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenMissingBothIngredientsAndSteps_ShouldReturnFalse()
    {
        // Arrange
        var recipe = new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            10, 20, 2, Difficulty.Easy);

        // Act
        var isValid = recipe.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenHasIngredientsButMissingSteps_ShouldReturnFalse()
    {
        // Arrange
        var recipe = new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            10, 20, 2, Difficulty.Easy);

        recipe.AddIngredient(new Ingredient("Sugar", 2, "tbsp"));

        // Act
        var isValid = recipe.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenHasStepsButMissingIngredients_ShouldReturnFalse()
    {
        // Arrange
        var recipe = new Recipe(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            10, 20, 2, Difficulty.Easy);

        recipe.AddStep(new Step("Mix everything together"));

        // Act
        var isValid = recipe.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }
}