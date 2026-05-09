using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionById;

public class GetAllergyRestrictionByIdQueryHandler : IRequestHandler<GetAllergyRestrictionByIdQuery, AllergyRestrictionDetailResponse>
{
    private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;
    private readonly IMapper _mapper;

    public GetAllergyRestrictionByIdQueryHandler(
        IAllergyRestrictionRepository allergyRestrictionRepository,
        AllergyRestrictionBusinessRules businessRules,
        IMapper mapper)
    {
        _allergyRestrictionRepository = allergyRestrictionRepository;
        _businessRules = businessRules;
        _mapper = mapper;
    }

    public async Task<AllergyRestrictionDetailResponse> Handle(GetAllergyRestrictionByIdQuery request, CancellationToken cancellationToken)
    {
        var restriction = await _allergyRestrictionRepository.GetAsync(
            predicate: ar => ar.Id == request.Id,
            include: q => q
                .Include(ar => ar.ExcludedIngredients).ThenInclude(ei => ei.Ingredient)
                .Include(ar => ar.ExcludedCategories).ThenInclude(ec => ec.Category),
            enableTracking: false,
            cancellationToken: cancellationToken);

        _businessRules.CheckIfAllergyRestrictionExists(restriction);

        return _mapper.Map<AllergyRestrictionDetailResponse>(restriction!);
    }
}
