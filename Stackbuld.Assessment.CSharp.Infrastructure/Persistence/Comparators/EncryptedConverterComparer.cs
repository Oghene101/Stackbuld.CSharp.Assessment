using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Comparators;

public class EncryptedConverterComparer() : ValueComparer<string>(
    (l, r) => string.Equals(l, r),
    v => v == null ? 0 : v.GetHashCode(),
    v => v)
{
}