﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Theatre.Domain.Aggregates.Actors;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Domain.Primitives;

namespace Theatre.Domain.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(contract => contract.Id);

        builder.Property(contract => contract.YearCost)
            .HasConversion(
                cost => cost.Amount,
                value => Money.Create(value).Value);

        builder.HasOne<Actor>()
            .WithMany()
            .HasForeignKey(contract => contract.ActorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(contract => contract.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(show => show.Transactions)
            .WithOne()
            .HasForeignKey(role => role.ContractId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}