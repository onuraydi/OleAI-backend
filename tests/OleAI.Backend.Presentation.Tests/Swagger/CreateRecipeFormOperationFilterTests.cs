using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OleAI.Backend.Presentation.Controllers;
using OleAI.Backend.Presentation.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Swagger;

public class CreateRecipeFormOperationFilterTests
{
    [Fact]
    public void Apply_WhenMethodIsAddInRecipesController_ShouldModifyRequestBody()
    {
        // Arrange
        var filter = new CreateRecipeFormOperationFilter();
        var operation = new OpenApiOperation();
        var methodInfo = typeof(RecipesController).GetMethod("Add");
        var context = new OperationFilterContext(null, null, null, methodInfo);

        // Act
        filter.Apply(operation, context);

        // Assert
        operation.RequestBody.Should().NotBeNull();
        operation.RequestBody.Content.Should().ContainKey("multipart/form-data");
        var schema = operation.RequestBody.Content["multipart/form-data"].Schema;
        schema.Properties.Should().ContainKey("MainImage");
    }

    [Fact]
    public void Apply_WhenMethodIsUpdateInRecipesController_ShouldModifyRequestBody()
    {
        // Arrange
        var filter = new CreateRecipeFormOperationFilter();
        var operation = new OpenApiOperation();
        var methodInfo = typeof(RecipesController).GetMethod("Update");
        var context = new OperationFilterContext(null, null, null, methodInfo);

        // Act
        filter.Apply(operation, context);

        // Assert
        operation.RequestBody.Should().NotBeNull();
        operation.RequestBody.Content.Should().ContainKey("multipart/form-data");
        var schema = operation.RequestBody.Content["multipart/form-data"].Schema;
        schema.Properties.Should().ContainKey("NewMainImage");
    }
}
