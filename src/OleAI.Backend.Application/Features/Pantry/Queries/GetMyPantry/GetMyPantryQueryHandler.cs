using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetMyPantry
{
    public class GetMyPantryQueryHandler : IRequestHandler<GetMyPantryQuery, List<PantryItemResponse>>
    {
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IMapper _mapper;

        public GetMyPantryQueryHandler(IUserIngredientRepository userIngredientRepository, IMapper mapper)
        {
            _userIngredientRepository = userIngredientRepository;
            _mapper = mapper;
        }

        public async Task<List<PantryItemResponse>> Handle(GetMyPantryQuery command, CancellationToken cancellationToken)
        {
            QueryOptions<UserIngredient> query = new QueryOptions<UserIngredient>();
            query.Predicate = ui => ui.UserId == command.UserId;
            query.Include = q => q.Include(ui => ui.Ingredient!).ThenInclude(i => i.Category).Include(ui => ui.IngredientRequest!);
            query.EnableTracking = false;

            var pantryList = await _userIngredientRepository.GetListAsync(query, cancellationToken);

            return _mapper.Map<List<PantryItemResponse>>(pantryList.Items);
        }
    }
}
