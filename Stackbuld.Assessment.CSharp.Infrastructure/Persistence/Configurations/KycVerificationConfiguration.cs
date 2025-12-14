using LoanApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Converters;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Configurations;

public class KycVerificationConfiguration(
    IEncryptionProvider encryptionProvider) : IEntityTypeConfiguration<KycVerification>
{
    public void Configure(EntityTypeBuilder<KycVerification> builder)
    {
        builder.HasIndex(k => k.UserId).IsUnique();
        builder.HasIndex(k => k.BvnHash).IsUnique();
        builder.HasIndex(k => k.NinHash).IsUnique();
        builder.Property(k => k.BvnCipher).HasConversion(new EncryptedConverter(encryptionProvider));
        builder.Property(k => k.NinCipher).HasConversion(new EncryptedConverter(encryptionProvider));
    }
}