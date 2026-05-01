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

        var formFields = new Dictionary<string, StringValues>
        {
            { "Title", "Test Recipe" },
            { "Description", "Delicious" },
            { "Visibility", "Public" },
            { "IngredientsJson", "[]" },
            { "StepsJson", "[]" }
        };

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Name).Returns("MainImage");
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        var files = new FormFileCollection { fileMock.Object };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.ContentType = "multipart/form-data";
        httpContext.Request.Form = new FormCollection(formFields, files);

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        };

        var context = new DefaultModelBindingContext
        {
            ModelName = "test",
            ModelState = new ModelStateDictionary(),
            ValueProvider = new Mock<IValueProvider>().Object,
            ActionContext = actionContext,
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(object))
        };

        // Act
        await binder.BindModelAsync(context);

        // Assert
        context.Result.IsModelSet.Should().BeTrue();
        context.ModelState.IsValid.Should().BeTrue();
    }
}
