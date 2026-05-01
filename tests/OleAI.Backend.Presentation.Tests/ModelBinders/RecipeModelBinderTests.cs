using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Moq;
using OleAI.Backend.Presentation.ModelBinders;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.ModelBinders;

public class RecipeModelBinderTests
{
    [Fact]
    public async Task CreateRecipeCommandModelBinder_WithValidForm_ShouldBindSuccessfully()
    {
        // Arrange
        var binder = new CreateRecipeCommandModelBinder();
        var context = CreateBindingContext("multipart/form-data", new Dictionary<string, StringValues>
        {
            { "Title", "Test Recipe" },
            { "Description", "Delicious" },
            { "Visibility", "Public" },
            { "IngredientsJson", "[]" },
            { "StepsJson", "[]" }
        }, "MainImage");

        // Act
        await binder.BindModelAsync(context);

        // Assert
        context.Result.IsModelSet.Should().BeTrue();
        context.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRecipeCommandModelBinder_WithValidForm_ShouldBindSuccessfully()
    {
        // Arrange
        var binder = new UpdateRecipeCommandModelBinder();
        var context = CreateBindingContext("multipart/form-data", new Dictionary<string, StringValues>
        {
            { "Title", "Updated Recipe" },
            { "Description", "Better" },
            { "Visibility", "Private" },
            { "IngredientsJson", "[]" },
            { "StepsJson", "[]" }
        }, "NewMainImage");

        // Act
        await binder.BindModelAsync(context);

        // Assert
        context.Result.IsModelSet.Should().BeTrue();
        context.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task BaseRecipeCommandModelBinder_WithInvalidJson_ShouldHaveModelStateError()
    {
        // Arrange
        var binder = new CreateRecipeCommandModelBinder();
        var context = CreateBindingContext("multipart/form-data", new Dictionary<string, StringValues>
        {
            { "Title", "Test" },
            { "IngredientsJson", "invalid-json" }
        }, "MainImage");

        // Act
        await binder.BindModelAsync(context);

        // Assert
        context.ModelState.ContainsKey("IngredientsJson").Should().BeTrue();
        context.ModelState["IngredientsJson"]!.Errors.Should().NotBeEmpty();
    }

    private static DefaultModelBindingContext CreateBindingContext(string contentType, Dictionary<string, StringValues> formFields, string? fileName = null)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.ContentType = contentType;

        IFormFileCollection files = new FormFileCollection();
        if (fileName != null)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Name).Returns(fileName);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            files = new FormFileCollection { fileMock.Object };
        }

        httpContext.Request.Form = new FormCollection(formFields, files);

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        };

        return new DefaultModelBindingContext
        {
            ModelName = "test",
            ModelState = new ModelStateDictionary(),
            ValueProvider = new Mock<IValueProvider>().Object,
            ActionContext = actionContext,
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(object))
        };
    }
}
