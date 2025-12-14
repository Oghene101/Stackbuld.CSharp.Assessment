using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stackbuld.Assessment.CSharp.Domain.Entities;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasIndex(a => a.KycVerificationId);
        builder.HasIndex(a => new
            {
                a.KycVerificationId, a.HouseNumber, a.Street, a.City, a.State, a.Country
            })
            .IsUnique();
    }
}