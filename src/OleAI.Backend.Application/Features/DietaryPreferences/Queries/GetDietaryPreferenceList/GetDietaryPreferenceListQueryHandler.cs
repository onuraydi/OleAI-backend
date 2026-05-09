using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceList;

public class GetDietaryPreferenceListQueryHandler : IRequestHandler<GetDietaryPreferenceListQuery, List<DietaryPreferenceListResponse>>
{
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
    private readonly IMapper _mapper;

    public GetDietaryPreferenceListQueryHandler(IDietaryPreferenceRepository dietaryPreferenceRepository, IMapper mapper)
    {
        _dietaryPreferenceRepository = dietaryPreferenceRepository;
        _mapper = mapper;
    }

    public async Task<List<DietaryPreferenceListResponse>> Handle(GetDietaryPreferenceListQuery request, CancellationToken cancellationToken)
    {
        var preferences = await _dietaryPreferenceRepository.GetListAsync(
            new Core.Repositories.Options.QueryOptions<Domain.Entities.DietaryPreference>
            {
                Include = q => q.Include(dp => dp.ExcludedIngredients).Include(dp => dp.ExcludedCategories),
                EnableTracking = false,
                Size = 1000
            },
            cancellationToken: cancellationToken);

        return _mapper.Map<List<DietaryPreferenceListResponse>>(preferences.Items);
    }
}
