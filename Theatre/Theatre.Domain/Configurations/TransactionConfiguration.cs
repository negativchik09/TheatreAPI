using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Theatre.Domain.Aggregates.Actors;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Domain.Primitives;

namespace Theatre.Domain.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(transaction => transaction.Id);
        
        builder.Property(transaction => transaction.Sum)
            .HasConversion(
                cost => cost.Amount,
                value => Money.Create(value).Value);

        builder.HasOne<Actor>()
            .WithMany()
            .HasForeignKey(contract => contract.ActorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}