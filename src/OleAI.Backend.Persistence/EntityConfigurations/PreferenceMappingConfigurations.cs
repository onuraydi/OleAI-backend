using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.Configurations;

public class UserDietaryPreferenceConfiguration : IEntityTypeConfiguration<UserDietaryPreference>
{
    public void Configure(EntityTypeBuilder<UserDietaryPreference> builder)
    {
        builder.HasKey(udp => new { udp.UserId, udp.DietaryPreferenceId });
    }
}

public class UserAllergyConfiguration : IEntityTypeConfiguration<UserAllergy>
{
    public void Configure(EntityTypeBuilder<UserAllergy> builder)
    {
        builder.HasKey(ua => new { ua.UserId, ua.AllergyRestrictionId });
    }
}

public class DietaryPreferenceExcludedIngredientConfiguration : IEntityTypeConfiguration<DietaryPreferenceExcludedIngredient>
{
    public void Configure(EntityTypeBuilder<DietaryPreferenceExcludedIngredient> builder)
    {
        builder.HasKey(x => new { x.DietaryPreferenceId, x.IngredientId });
    }
}

public class DietaryPreferenceExcludedCategoryConfiguration : IEntityTypeConfiguration<DietaryPreferenceExcludedCategory>
{
    public void Configure(EntityTypeBuilder<DietaryPreferenceExcludedCategory> builder)
    {
        builder.HasKey(x => new { x.DietaryPreferenceId, x.CategoryId });
    }
}

public class AllergyExcludedIngredientConfiguration : IEntityTypeConfiguration<AllergyExcludedIngredient>
{
    public void Configure(EntityTypeBuilder<AllergyExcludedIngredient> builder)
    {
        builder.HasKey(x => new { x.AllergyRestrictionId, x.IngredientId });
    }
}

public class AllergyExcludedCategoryConfiguration : IEntityTypeConfiguration<AllergyExcludedCategory>
{
    public void Configure(EntityTypeBuilder<AllergyExcludedCategory> builder)
    {
        builder.HasKey(x => new { x.AllergyRestrictionId, x.CategoryId });
    }
}
