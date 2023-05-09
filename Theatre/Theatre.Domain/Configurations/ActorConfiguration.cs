using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Theatre.Domain.Aggregates.Actors;

namespace Theatre.Domain.Configurations;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.HasKey(a => a.Id);

        builder.OwnsOne(x => x.Name);
    }
}