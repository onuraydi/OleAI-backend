namespace OleAI.Backend.Application.Features.Recipes.Queries;

public interface IPageableRequest
{
    int Page { get; set; }
    int Size { get; set; }
}
