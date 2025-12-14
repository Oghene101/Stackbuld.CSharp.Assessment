using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stackbuld.Assessment.CSharp.Application.Common.Contracts.Abstractions;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence.Converters;

public class EncryptedConverter(
    IEncryptionProvider encryptionProvider) : ValueConverter<string, string>(
    v => encryptionProvider.Encrypt(v),
    v => encryptionProvider.Decrypt(v))
{
}