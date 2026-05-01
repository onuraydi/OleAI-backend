using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OleAI.Backend.Presentation.Swagger;

public class CreateRecipeFormOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var isCreate = string.Equals(context.MethodInfo.Name, "Add", StringComparison.Ordinal);
        var isUpdate = string.Equals(context.MethodInfo.Name, "Update", StringComparison.Ordinal);

        if ((!isCreate && !isUpdate) || context.MethodInfo.DeclaringType?.Name != "RecipesController")
        {
            return;
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Required = BuildRequiredFields(isCreate),
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["Title"] = new OpenApiSchema { Type = "string" },
                            ["Description"] = new OpenApiSchema { Type = "string" },
                            ["Visibility"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Public") },
                            [isCreate ? "MainImage" : "NewMainImage"] = new OpenApiSchema { Type = "string", Format = "binary" },
                            ["IngredientsJson"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("[{\"ingredientId\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"amount\":1,\"unitId\":1}]") },
                            ["StepsJson"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("[{\"stepNumber\":1,\"instruction\":\"Adım 1\"}]") },
                            ["StepImages"] = new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string", Format = "binary" } },
                            ["StepImageStepNumbers"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("1,2") }
                        }
                    }
                }
            }
        };

        if (operation.Parameters?.Count > 0)
        {
            operation.Parameters = operation.Parameters
                .Where(p => p.In == ParameterLocation.Path)
                .ToList();
        }
    }

    private static HashSet<string> BuildRequiredFields(bool isCreate)
    {
        var required = new HashSet<string>
        {
            "Title",
            "Description",
            "Visibility",
            "IngredientsJson",
            "StepsJson"
        };

        if (isCreate)
        {
            required.Add("MainImage");
        }

        return required;
    }
}
