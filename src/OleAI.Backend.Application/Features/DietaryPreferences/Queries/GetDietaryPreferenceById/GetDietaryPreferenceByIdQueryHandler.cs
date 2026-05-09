using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.DietaryPreferences.Dtos.Responses;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceById;

public class GetDietaryPreferenceByIdQueryHandler : IRequestHandler<GetDietaryPreferenceByIdQuery, DietaryPreferenceDetailResponse>
{
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;
    private readonly IMapper _mapper;

    public GetDietaryPreferenceByIdQueryHandler(
        IDietaryPreferenceRepository dietaryPreferenceRepository,
        DietaryPreferenceBusinessRules businessRules,
        IMapper mapper)
    {
        _dietaryPreferenceRepository = dietaryPreferenceRepository;
        _businessRules = businessRules;
        _mapper = mapper;
    }

    public async Task<DietaryPreferenceDetailResponse> Handle(GetDietaryPreferenceByIdQuery request, CancellationToken cancellationToken)
    {
        var preference = await _dietaryPreferenceRepository.GetAsync(
            predicate: dp => dp.Id == request.Id,
            include: q => q
                .Include(dp => dp.ExcludedIngredients).ThenInclude(ei => ei.Ingredient)
                .Include(dp => dp.ExcludedCategories).ThenInclude(ec => ec.Category),
            enableTracking: false,
            cancellationToken: cancellationToken);

        _businessRules.CheckIfDietaryPreferenceExists(preference);

        return _mapper.Map<DietaryPreferenceDetailResponse>(preference!);
    }
}
