using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Profiles;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Profiles;

public class IngredientAliasMappingProfileTests
{
    private readonly IMapper _mapper;

    public IngredientAliasMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<IngredientAliasMappingProfile>());
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Construct_ShouldHaveValidConfiguration()
    {
        var alias = new IngredientAlias(Guid.NewGuid(), Guid.NewGuid(), "Domat");
        var create = new CreateIngredientAliasRequest { IngredientId = Guid.NewGuid(), AliasName = "Patlican" };
        var update = new UpdateIngredientAliasRequest { Id = alias.Id, IngredientId = alias.IngredientId, AliasName = "Domates" };

        var act1 = () => _mapper.Map<IngredientAliasResponse>(alias);
        var act2 = () => _mapper.Map<IngredientAlias>(create);
        var act3 = () => _mapper.Map(update, alias);

        act1.Should().NotThrow();
        act2.Should().NotThrow();
        act3.Should().NotThrow();
    }
}
