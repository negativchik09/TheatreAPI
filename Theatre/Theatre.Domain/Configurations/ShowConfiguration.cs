using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Domain.Primitives;

namespace Theatre.Domain.Configurations;

public class ShowConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        builder.HasKey(show => show.Id);

        builder.Property(show => show.TotalBudget)
            .HasConversion(
                cost => cost.Amount,
                value => Money.Create(value).Value);
        
        builder.Property(show => show.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasMany(show => show.Roles)
            .WithOne()
            .HasForeignKey(role => role.ShowId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasMany(show => show.Contracts)
            .WithOne()
            .HasForeignKey(role => role.ShowId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}