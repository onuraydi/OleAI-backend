using MediatR;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Dtos;
namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetCategories;

public class GetRecipeCategoriesQuery : IRequest<List<RecipeCategoryDto>> { }
