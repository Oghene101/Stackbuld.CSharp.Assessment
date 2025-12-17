using System.Reflection;
using LoanApplication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;
using Stackbuld.Assessment.CSharp.Domain.Attributes;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Comparators;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Configurations;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Converters;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<KycVerification> KycVerifications { get; set; }
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        var encryptionProvider = this.GetService<IEncryptionProvider>();
        modelBuilder.ApplyConfiguration(new KycVerificationConfiguration(encryptionProvider));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(string)) continue;

                var memberInfo = property.PropertyInfo ?? (MemberInfo)property.FieldInfo;
                if (memberInfo == null || !Attribute.IsDefined(memberInfo, typeof(EncryptedAttribute))) continue;

                property.SetValueConverter(new EncryptedConverter(encryptionProvider));
                property.SetValueComparer(new EncryptedConverterComparer());
            }
        }
    }
}