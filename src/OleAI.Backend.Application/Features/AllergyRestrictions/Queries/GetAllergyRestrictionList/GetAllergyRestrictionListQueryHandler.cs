using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.AllergyRestrictions.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionList;

public class GetAllergyRestrictionListQueryHandler : IRequestHandler<GetAllergyRestrictionListQuery, List<AllergyRestrictionListResponse>>
{
    private readonly IAllergyRestrictionRepository _allergyRestrictionRepository;
    private readonly IMapper _mapper;

    public GetAllergyRestrictionListQueryHandler(IAllergyRestrictionRepository allergyRestrictionRepository, IMapper mapper)
    {
        _allergyRestrictionRepository = allergyRestrictionRepository;
        _mapper = mapper;
    }

    public async Task<List<AllergyRestrictionListResponse>> Handle(GetAllergyRestrictionListQuery request, CancellationToken cancellationToken)
    {
        var restrictions = await _allergyRestrictionRepository.GetListAsync(
            new Core.Repositories.Options.QueryOptions<Domain.Entities.AllergyRestriction>
            {
                Include = q => q.Include(ar => ar.ExcludedIngredients).Include(ar => ar.ExcludedCategories),
                EnableTracking = false,
                WithDeleted = true,
                Size = 1000
            },
            cancellationToken: cancellationToken);

        return _mapper.Map<List<AllergyRestrictionListResponse>>(restrictions.Items);
    }
}
