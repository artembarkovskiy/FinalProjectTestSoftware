using System;
using AutoFixture;
using Core;
using FluentAssertions;
using Xunit;

namespace Unit.Entities;

public class StepTests
{
    private readonly IFixture _fixture;

    public StepTests()
    {
        _fixture = new Fixture();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenInstructionIsNullOrWhiteSpace_ShouldThrowArgumentException(string invalidInstruction)
    {
        // Act
        Action act = () => new Step(invalidInstruction);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Instruction cannot be empty*");
    }

    [Fact]
    public void Constructor_WithValidData_ShouldCreateStep()
    {
        // Arrange
        var instruction = "Chop the onions finely.";

        // Act
        var step = new Step(instruction);

        // Assert
        step.Should().NotBeNull();
        step.Instruction.Should().Be(instruction);
        step.Id.Should().NotBeEmpty();
        
        step.StepNumber.Should().Be(0); 
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueId()
    {
        // Act
        var step1 = new Step("Boil water");
        var step2 = new Step("Boil water");

        // Assert
        step1.Id.Should().NotBe(step2.Id);
    }

    
}