using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Theatre.Domain.Aggregates.Shows;

namespace Theatre.Domain.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);

        builder.HasOne<Show>()
            .WithMany(x => x.Roles)
            .HasForeignKey(role => role.ShowId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(role => role.Title).HasMaxLength(255);
    }
}