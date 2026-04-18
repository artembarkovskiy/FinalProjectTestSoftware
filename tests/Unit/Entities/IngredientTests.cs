using System;
using AutoFixture;
using Core; 
using FluentAssertions;
using Xunit;

namespace Unit.Entities;

public class IngredientTests
{
    private readonly IFixture _fixture;

    public IngredientTests()
    {
        _fixture = new Fixture();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1.5)]
    [InlineData(-100)]
    public void Constructor_WhenQuantityIsInvalid_ShouldThrowArgumentException(decimal invalidQuantity)
    {
        // Act
        Action act = () => new Ingredient(
            _fixture.Create<string>(), 
            invalidQuantity, 
            _fixture.Create<string>());

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Quantity must be positive*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNameIsNullOrWhiteSpace_ShouldThrowArgumentException(string invalidName)
    {
        // Act
        Action act = () => new Ingredient(
            invalidName, 
            1.5m, 
            "kg");

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Ingredient name cannot be empty*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenUnitIsNullOrWhiteSpace_ShouldThrowArgumentException(string invalidUnit)
    {
        // Act
        Action act = () => new Ingredient(
            "Tomato", 
            2m, 
            invalidUnit);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Unit cannot be empty*");
    }

    [Fact]
    public void Constructor_WithValidData_ShouldCreateIngredient()
    {
        // Arrange
        var name = "Sugar";
        var quantity = 2.5m;
        var unit = "tbsp";

        // Act
        var ingredient = new Ingredient(name, quantity, unit);

        // Assert
        ingredient.Should().NotBeNull();
        ingredient.Name.Should().Be(name);
        ingredient.Quantity.Should().Be(quantity);
        ingredient.Unit.Should().Be(unit);
        ingredient.Id.Should().NotBeEmpty();
    }
    
    [Fact]
    public void Constructor_WithExtremelySmallPositiveQuantity_ShouldCreateIngredient()
    {
        // Arrange
        var verySmallQuantity = 0.001m; 

        // Act
        var ingredient = new Ingredient("Saffron", verySmallQuantity, "g");

        // Assert
        ingredient.Should().NotBeNull();
        ingredient.Quantity.Should().Be(verySmallQuantity);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueId()
    {
        // Act
        var ingredient1 = new Ingredient("Tomato", 2m, "kg");
        var ingredient2 = new Ingredient("Tomato", 2m, "kg");

        // Assert
        ingredient1.Id.Should().NotBe(ingredient2.Id);
        ingredient1.Id.Should().NotBeEmpty();
    }
}