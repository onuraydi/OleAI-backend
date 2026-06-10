using MediatR;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Dtos;
namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetTags;

public class GetTagsQuery : IRequest<List<TagDto>> { }
